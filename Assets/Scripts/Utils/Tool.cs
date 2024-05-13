using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public static class Tool
{
    private static Dictionary<GameObject, TowerCaract> towerInGame = new Dictionary<GameObject, TowerCaract>();
    private static Dictionary<EnemyType, EnemyWaveCaract> nextWavePower = new Dictionary<EnemyType, EnemyWaveCaract>();

    #region PowerTowerWave
    public static void AddTilePower(this SCO_TileData tileData)
    {
        if (towerInGame.ContainsKey(tileData.tilePrefabs))
        {
            towerInGame[tileData.tilePrefabs].numberTower += 1;
        }
        else
        {
            towerInGame.Add(tileData.tilePrefabs, new TowerCaract(tileData.powerTile, tileData.tileEnemyAccess));
        }
    }

    public static void RemoveTilePower(this SCO_TileData tileData)
    {
        if (towerInGame[tileData.tilePrefabs].numberTower - 1 <= 0) { towerInGame.Remove(tileData.tilePrefabs); return; }
        towerInGame[tileData.tilePrefabs].numberTower -= 1;
    }

    public static float GetTowersPow()
    {
        return towerInGame.Sum(o => o.Value.powerTower * o.Value.numberTower);
    }
    #endregion

    #region PowerEnemyWave
    public static void PickPowerWave(this EnemyWave enemyWave)
    {
        if (nextWavePower.ContainsKey(enemyWave.enemyType))
        {
            nextWavePower[enemyWave.enemyType].numberUnit += 1;
        }
        else
        {
            nextWavePower.Add(enemyWave.enemyType, new EnemyWaveCaract(enemyWave.powerUnit));
        }
    }

    public static void ResetWavePower() { nextWavePower.Clear(); }

    public static float GetWavePow()
    {
        return nextWavePower.Sum(o => o.Value.powerUnit * o.Value.numberUnit);
    }
    #endregion

    #region Tool
    public static void Bump(this Transform tr, float scale)
    {
        tr.DOScale(scale, .08f).OnComplete(() => {
            tr.DOScale(1, .07f);
        });
    }

    public static T GetRandom<T>(this IEnumerable<T> elems)
    {
        if (elems.Count() == 0) throw new ArgumentException("List empty");
        return elems.ElementAt(new System.Random().Next(0, elems.Count()));
    }

    public static bool FindIn<T>(this IEnumerable<T> elems, T elemFind)
    {
        foreach (var elem in elems) if (EqualityComparer<T>.Default.Equals(elemFind)) return true;
        return false;
    }

    public static SCO_TileData[] ShuffleHand(this List<SCO_TileData> tiles, int nbTileHand)
    {

        SCO_TileData[] hand = new SCO_TileData[nbTileHand];
        int index = 0;
        int nbEnemies = nextWavePower.Values.ToList().Sum(o => o.numberUnit);

        var typeEnemiesPresent = nextWavePower.Keys.ToList();

        float differencePow = GetWavePow() - GetTowersPow();

        System.Random rnd = new System.Random();

        //Start sort data
        var tileCanPickAll = tiles.OrderBy(data => data.powerTile).ThenBy(o => rnd.Next()).ToList();

        bool pass = true;
        for (int i = tileCanPickAll.Count - 1; i >= 0; --i)
        {
            pass = false;
            foreach (var typeTile in tileCanPickAll[i].tileEnemyAccess)
            {
                if (typeTile == EnemyType.None)
                {
                    pass = true;
                }
                else if (pass == false)
                {
                    pass = typeEnemiesPresent.Contains(typeTile);
                }
            }
            if (!pass) tileCanPickAll.RemoveAt(i);
        }

        List<EnemyType> typeMissing = new List<EnemyType>();

        foreach (EnemyType type in nextWavePower.Keys)
        {
            pass = false;
            foreach (TowerCaract towerCaract in towerInGame.Values)
            {
                pass = towerCaract.enemyTypesAccess.Contains(type);
                if (pass) break;
            }
            if (towerInGame.Count == 0) typeMissing.Add(type);
            else if (pass == true)
            {
                typeMissing.Add(type);
            }
        }

        List<SCO_TileData> tileCanPickTower = new List<SCO_TileData>();
        foreach (SCO_TileData tile in tileCanPickAll)
        {
            foreach (EnemyType type in typeMissing)
            {
                if (tile.tileEnemyAccess.Contains(type))
                {
                    tileCanPickTower.Add(tile);
                    break;
                }
            }
        }
        //End sort data

        //Check to add tower need
        for (int i = 0; i < ((typeMissing.Count > nbTileHand * 0.3) ? nbTileHand * 0.3 : typeMissing.Count); ++i)
        {
            EnemyType type = typeMissing.First();
            foreach (var tile in tileCanPickTower.FindAll(o => o.tileEnemyAccess.Contains(type)))
            {
                if (tile.powerTile > differencePow) continue;
                differencePow -= Mathf.Round(tile.powerTile);
                hand[index] = tile;
                index++;
                typeMissing.Remove(type);
                break;
            }
        }
        //End check

        while (tileCanPickAll.Count > 0 && index < nbTileHand)
        {
            ClearDataByPowerRemain(ref tileCanPickAll, ref differencePow);
            var tile = (differencePow < 1)? tileCanPickAll.GetRandom():GetRandomTileByChance(ref tileCanPickAll);
            if (tile == null) { Debug.LogWarning("Tile picked null"); continue; }
            hand[index] = tile;
            index++;
            differencePow -= tile.powerTile;
        }
        return hand;
    }

    private static void ClearDataByPowerRemain(ref List<SCO_TileData> tileCanPick, ref float diffPower)
    {
        for (int i = tileCanPick.Count-1; i >=0; --i)
        {
            if (tileCanPick[i].powerTile > diffPower) tileCanPick.RemoveAt(i);
        }
    }

    private static SCO_TileData GetRandomTileByChance(ref List<SCO_TileData> tiles)
    {
        float ttRarety = 0;
        foreach (SCO_TileData tile in tiles)
        {
            ttRarety += tile.powerTile;
        }


        float rndNumber = UnityEngine.Random.Range(0, ttRarety+1);

        float cumulRarety = 0;

        foreach(SCO_TileData tile in tiles)
        {
            cumulRarety += tile.powerTile;
            if (rndNumber < cumulRarety) return tile;
        }

        return null;
    }

    public static void ResetTool()
    {
        towerInGame.Clear();
        nextWavePower.Clear();
    }

    public static IEnumerator Delay(Action ev, float delay)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }

    #endregion Tool

    #region InternalClass
    private class EnemyWaveCaract
    {
        public EnemyWaveCaract(float powerUnit)
        {
            this.powerUnit = powerUnit;
            numberUnit = 1;
        }

        public int numberUnit;
        public float powerUnit;
    }

    private class TowerCaract
    {
        public TowerCaract(float powerTower, EnemyType[] enemyTypes)
        {
            this.powerTower = powerTower;
            this.enemyTypesAccess = enemyTypes;
            numberTower = 1;
        }

        public int numberTower;
        public float powerTower;
        public EnemyType[] enemyTypesAccess;
    }
    #endregion
}