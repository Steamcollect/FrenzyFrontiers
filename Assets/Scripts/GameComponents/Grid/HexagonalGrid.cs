using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexagonalGrid : MonoBehaviour
{
    public float horizontalDistance;
    public List<Hexagone> hexagones = new List<Hexagone>();
    public List<int> hexagoneTiles = new List<int>();
    public List<int> placementTiles = new List<int>();
    public float farthestHexagoneDistance;

    public GameObject tileGo;
    public GameObject vectorGo;

    DefensesManager defensesManager;

    private void Awake()
    {
        defensesManager = FindFirstObjectByType<DefensesManager>();
    }

    public void CreateNewTile(int placementIndex, GameObject go, SCO_TileData data)
    {
        // Set the tile in Hex grid
        hexagoneTiles.Add(placementIndex);
        placementTiles.Remove(placementIndex);

        // Set visual
        if(hexagones[placementIndex].tileVisual != null) hexagones[placementIndex].tileVisual.SetActive(false);
        hexagones[placementIndex].hexGO = go;
        hexagones[placementIndex].hexGO.transform.SetParent(transform);

        // Set data
        hexagones[placementIndex].tileData = data;

        // Is defense tile
        if (hexagones[placementIndex].tileData.typeTile == TypeTile.Tower)
            defensesManager.defences.Add(hexagones[placementIndex].hexGO.GetComponent<Defenses>());

        hexagones[placementIndex].hexGO.GetComponent<Tile>().indexInGrid = placementIndex;
        LifeTileComponent lifeTileComponent = hexagones[placementIndex].hexGO.GetComponent<LifeTileComponent>();

        lifeTileComponent.indexInGrid = placementIndex;
        lifeTileComponent.onDeath += RemoveHex;

        if(hexagones[placementIndex].hexGO.TryGetComponent<Defenses>(out Defenses defense))
        {
            defense.detectionRangeVisual.SetActive(false);
        }
        

        CreateHexPos(hexagones[placementIndex]);

        // Do bump effect
        hexagones[placementIndex].hexGO.transform.DOScale(1.08f, .07f).OnComplete(()=>{
            hexagones[placementIndex].hexGO.transform.DOScale(1f, .07f); });
    }
    public void RemoveHex(int hexagoneIndex)
    {
        if (hexagoneIndex != 0)
        {
            hexagones[hexagoneIndex].tileVisual.transform.localScale = Vector3.zero;
            hexagones[hexagoneIndex].tileVisual.SetActive(true);
        }
        hexagones[hexagoneIndex].hexGO.transform.DOKill();
        Destroy(hexagones[hexagoneIndex].hexGO);

        placementTiles.Add(hexagoneIndex);
        hexagoneTiles.Remove(hexagoneIndex);
    }

    public void CreateHexPos(Hexagone currentHex)
    {
        int currentHexIndex = FindHexagoneWithCubePos(currentHex.cubePos);

        // Set tmp hex references
        TmpPlacementHex[] tmpPlacementHex = new TmpPlacementHex[6];
        for (int i = 0; i < tmpPlacementHex.Length; i++)
        {
            tmpPlacementHex[i].hexagoneGridIndex = -1;
            tmpPlacementHex[i].placementHexIndex = -1;
        }

        // create cube position
        tmpPlacementHex[0].cubePos = new Vector3(currentHex.cubePos.x - 1, currentHex.cubePos.y, currentHex.cubePos.z + 1);
        tmpPlacementHex[1].cubePos = new Vector3(currentHex.cubePos.x, currentHex.cubePos.y - 1, currentHex.cubePos.z + 1);
        tmpPlacementHex[2].cubePos = new Vector3(currentHex.cubePos.x + 1, currentHex.cubePos.y - 1, currentHex.cubePos.z);
        tmpPlacementHex[3].cubePos = new Vector3(currentHex.cubePos.x + 1, currentHex.cubePos.y, currentHex.cubePos.z - 1);
        tmpPlacementHex[4].cubePos = new Vector3(currentHex.cubePos.x, currentHex.cubePos.y + 1, currentHex.cubePos.z - 1);
        tmpPlacementHex[5].cubePos = new Vector3(currentHex.cubePos.x - 1, currentHex.cubePos.y + 1, currentHex.cubePos.z);

        // Find if hexagone already exist in this position
        for(int i = 0; i < hexagoneTiles.Count; i++)
        {
            for (int y = 0; y < 6; y++)
            {
                if (hexagones[hexagoneTiles[i]].cubePos == tmpPlacementHex[y].cubePos) tmpPlacementHex[y].hexagoneGridIndex = i;
            }
        }
        // Find if placement already exist in this position
        for (int i = 0; i < placementTiles.Count; i++)
        {
            for (int y = 0; y < 6; y++)
            {
                if (hexagones[placementTiles[i]].cubePos == tmpPlacementHex[y].cubePos) tmpPlacementHex[y].placementHexIndex = i;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------//

        // Middle left
        if (tmpPlacementHex[0].hexagoneGridIndex > -1)
        {
            currentHex.nearestHexIndex[0] = tmpPlacementHex[0].hexagoneGridIndex;
            hexagones[tmpPlacementHex[0].hexagoneGridIndex].nearestHexIndex[3] = currentHexIndex;
        }
        else if (tmpPlacementHex[0].placementHexIndex == -1)
        {
            tmpPlacementHex[0].axialPos = new Vector3(currentHex.axialPos.x - horizontalDistance, 0, currentHex.axialPos.z);
            CreatePlacementTile(tmpPlacementHex[0].axialPos, tmpPlacementHex[0].cubePos);
        }

        // Top left
        if (tmpPlacementHex[1].hexagoneGridIndex > -1)
        {
            currentHex.nearestHexIndex[1] = tmpPlacementHex[1].hexagoneGridIndex;
            hexagones[tmpPlacementHex[1].hexagoneGridIndex].nearestHexIndex[4] = currentHexIndex;
        }
        else if (tmpPlacementHex[1].placementHexIndex == -1)
        {
            tmpPlacementHex[1].axialPos = new Vector3(currentHex.axialPos.x - (horizontalDistance / 2), 0, currentHex.axialPos.z + (horizontalDistance * Mathf.Sqrt(3) / 2));
            CreatePlacementTile(tmpPlacementHex[1].axialPos, tmpPlacementHex[1].cubePos);
        }

        // Top right
        if (tmpPlacementHex[2].hexagoneGridIndex > -1)
        {
            currentHex.nearestHexIndex[2] = tmpPlacementHex[2].hexagoneGridIndex;
            hexagones[tmpPlacementHex[2].hexagoneGridIndex].nearestHexIndex[5] = currentHexIndex;
        }
        else if (tmpPlacementHex[2].placementHexIndex == -1)
        {
            tmpPlacementHex[2].axialPos = new Vector3(currentHex.axialPos.x + (horizontalDistance / 2), 0, currentHex.axialPos.z + (horizontalDistance * Mathf.Sqrt(3) / 2));
            CreatePlacementTile(tmpPlacementHex[2].axialPos, tmpPlacementHex[2].cubePos);
        }

        // Middle right
        if (tmpPlacementHex[3].hexagoneGridIndex > -1)
        {
            currentHex.nearestHexIndex[3] = tmpPlacementHex[3].hexagoneGridIndex;
            hexagones[tmpPlacementHex[3].hexagoneGridIndex].nearestHexIndex[0] = currentHexIndex;
        }
        else if (tmpPlacementHex[3].placementHexIndex == -1)
        {
            tmpPlacementHex[3].axialPos = new Vector3(currentHex.axialPos.x + horizontalDistance, 0, currentHex.axialPos.z);
            CreatePlacementTile(tmpPlacementHex[3].axialPos, tmpPlacementHex[3].cubePos);
        }

        // Buttom right
        if (tmpPlacementHex[4].hexagoneGridIndex > -1)
        {
            currentHex.nearestHexIndex[4] = tmpPlacementHex[4].hexagoneGridIndex;
            hexagones[tmpPlacementHex[4].hexagoneGridIndex].nearestHexIndex[2] = currentHexIndex;
        }
        else if (tmpPlacementHex[4].placementHexIndex == -1)
        {
            tmpPlacementHex[4].axialPos = new Vector3(currentHex.axialPos.x + (horizontalDistance / 2), 0, currentHex.axialPos.z - (horizontalDistance * Mathf.Sqrt(3) / 2));
            CreatePlacementTile(tmpPlacementHex[4].axialPos, tmpPlacementHex[4].cubePos);
        }

        // Buttom left
        if (tmpPlacementHex[5].hexagoneGridIndex > -1)
        {
            currentHex.nearestHexIndex[5] = tmpPlacementHex[5].hexagoneGridIndex;
            hexagones[tmpPlacementHex[5].hexagoneGridIndex].nearestHexIndex[1] = currentHexIndex;
        }
        else if(tmpPlacementHex[5].placementHexIndex == -1)
        {
            tmpPlacementHex[5].axialPos = new Vector3(currentHex.axialPos.x - (horizontalDistance / 2), 0, currentHex.axialPos.z - (horizontalDistance * Mathf.Sqrt(3) / 2));
            CreatePlacementTile(tmpPlacementHex[5].axialPos, tmpPlacementHex[5].cubePos);
        }

        farthestHexagoneDistance = GetFarthestHexagone();
    }
    public void ResetPlacementInGrid()
    {
        for (int i = 0; i < placementTiles.Count; i++)
        {
            TmpPlacementHex[] tmpPlacementHex = new TmpPlacementHex[6];
            Hexagone currentHex = hexagones[placementTiles[i]];

            // Set every variables to null
            for (int y = 0; y < tmpPlacementHex.Length; y++)
            {
                tmpPlacementHex[y].hexagoneGridIndex = -1;
                tmpPlacementHex[y].placementHexIndex = -1;
            }

            // Create cube position
            tmpPlacementHex[0].cubePos = new Vector3(currentHex.cubePos.x - 1, currentHex.cubePos.y, currentHex.cubePos.z + 1);
            tmpPlacementHex[1].cubePos = new Vector3(currentHex.cubePos.x, currentHex.cubePos.y - 1, currentHex.cubePos.z + 1);
            tmpPlacementHex[2].cubePos = new Vector3(currentHex.cubePos.x + 1, currentHex.cubePos.y - 1, currentHex.cubePos.z);
            tmpPlacementHex[3].cubePos = new Vector3(currentHex.cubePos.x + 1, currentHex.cubePos.y, currentHex.cubePos.z - 1);
            tmpPlacementHex[4].cubePos = new Vector3(currentHex.cubePos.x, currentHex.cubePos.y + 1, currentHex.cubePos.z - 1);
            tmpPlacementHex[5].cubePos = new Vector3(currentHex.cubePos.x - 1, currentHex.cubePos.y + 1, currentHex.cubePos.z);

            // Set variable if there is hexagone in the placement
            for (int y = 0; y < hexagoneTiles.Count; y++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (hexagones[hexagoneTiles[y]].cubePos == tmpPlacementHex[j].cubePos) tmpPlacementHex[j].hexagoneGridIndex = y;
                }
            }

            if (tmpPlacementHex[0].hexagoneGridIndex == -1 &&
                tmpPlacementHex[1].hexagoneGridIndex == -1 &&
                tmpPlacementHex[2].hexagoneGridIndex == -1 &&
                tmpPlacementHex[3].hexagoneGridIndex == -1 &&
                tmpPlacementHex[4].hexagoneGridIndex == -1 &&
                tmpPlacementHex[5].hexagoneGridIndex == -1)
            {
                placementTiles.Remove(placementTiles[i]);
                i--;
            }
        }
    }

    void CreatePlacementTile(Vector3 axialPos, Vector3 cubePos)
    {
        GameObject tmpGO = null;

        tmpGO = Instantiate(vectorGo, axialPos, Quaternion.identity);
        tmpGO.transform.Bump(1.1f);
        tmpGO.transform.SetParent(transform);

        placementTiles.Add(hexagones.Count);
        hexagones.Add(new Hexagone(axialPos, cubePos, tmpGO, null, null));

        tmpGO.transform.DOScale(1.08f, .07f).OnComplete(() => { tmpGO.transform.DOScale(1f, .07f); });
        tmpGO.name += " " + (hexagoneTiles.Count - 1);
    }
    struct TmpPlacementHex
    {
        public Vector3 cubePos;
        public Vector3 axialPos;

        public int placementHexIndex;
        public int hexagoneGridIndex;
    }

    int FindHexagoneWithCubePos(Vector3 cubePos)
    {
        for (int i = 0; i < hexagoneTiles.Count; i++)
        {
            if (hexagones[hexagoneTiles[i]].cubePos == cubePos) return hexagoneTiles[i];
        }
        return -1;
    }
    int FindPlacementWithCubePos(Vector3 cubePos)
    {
        for (int i = 0; i < placementTiles.Count; i++)
        {
            if (hexagones[placementTiles[i]].cubePos == cubePos) return placementTiles[i];
        }
        return -1;
    }

    float GetFarthestHexagone()
    {
        float maxDist = 0;

        foreach (int current in hexagoneTiles)
        {
            float dist = Vector3.Distance(hexagones[current].axialPos, Vector3.zero);
            if (dist > maxDist) maxDist = dist;
        }

        return maxDist;
    }
    public int GetClosestHexagone(Vector3 currentPos)
    {
        int currentHexIndex = -1;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < hexagoneTiles.Count; i++)
        {
            //float dist = (currentPos - hexagones[hexagoneTiles[i]].axialPos).sqrMagnitude;
            float dist = Vector3.Distance(currentPos, hexagones[hexagoneTiles[i]].axialPos);

            if (dist < minDist)
            {
                currentHexIndex = hexagoneTiles[i];
                minDist = dist;
            }
        }
        return currentHexIndex;
    }

    public void SetActivePlacementHex(bool isActive)
    {
        Vector3 finalScale = isActive ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);

        foreach  (int current in placementTiles)
        {
            if (hexagones[current].tileVisual != null)
            {
                hexagones[current].tileVisual.transform.DOKill();
                hexagones[current].tileVisual.transform.DOScale(finalScale, .4f).SetEase(Ease.OutQuad);
            }
        }
    }
}

[System.Serializable]
public class Hexagone
{
    public Vector3 axialPos;
    public Vector3 cubePos;
    public GameObject tileVisual;
    public GameObject hexGO;
    public SCO_TileData tileData;

    public int[] nearestHexIndex = new int[6] { -1,-1,-1,-1,-1,-1};
    /*
    0 = middle left
    1 = top left
    2 = top right
    3 = middle right
    4 = buttom right
    5 = buttom left
    */

    public Hexagone(Vector3 axialPos, Vector3 cubePos, GameObject tileVisual, GameObject hexGO, SCO_TileData data)
    {
        this.axialPos = axialPos;
        this.cubePos = cubePos;
        this. tileVisual = tileVisual;
        this.hexGO = hexGO;
        tileData = data;
    }
}