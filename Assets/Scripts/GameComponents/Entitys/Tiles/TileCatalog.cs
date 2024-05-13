using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileCatalog : MonoBehaviour
{
    public List<SCO_TileData> tilesInInventory;
    public List<TilesUnlocked> tilesToUnlocked;
    public SCO_TileData hdvTile;

    private void Start()
    {
        if (tilesToUnlocked != null) tilesToUnlocked = tilesToUnlocked.OrderBy(o => o.waveUnlocked).ToList();
    }


    public void UpdateTilesInventory(int currentWave)
    {
        if (tilesToUnlocked == null || tilesToUnlocked.Count() == 0) return;
        for (int i = tilesToUnlocked.Count -1; i >= 0 ; i--)
        {
            if (tilesToUnlocked[i].waveUnlocked <= currentWave)
            {
                tilesInInventory.Add(tilesToUnlocked[i].tile);
                tilesToUnlocked.RemoveAt(i);
            }
        }
    }
}

[System.Serializable]
public class TilesUnlocked
{
    public SCO_TileData tile;
    public int waveUnlocked;
}