using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class TilePlacementManager : MonoBehaviour
{
    public Transform tilesVisualContent;
    public float tilesVisualSpacing;

    public int tileToPlaceCount = 5;

    GameObject nextTileToPlaceVisual;
    float tileToPlaceRotation = 0;

    List<Transform> tilesToPlaceVisual = new List<Transform>();
    List<SCO_TileData> tilesToPlace = new List<SCO_TileData>();

    public AudioClip[] tileMoveClips;
    public AudioClip[] tilePlaceClips;

    int currentPlacementSelectedIndex = -1;

    Plane plane = new Plane(Vector3.up, new Vector3(0, .2f, 0));

    Camera cam;
    HexagonalGrid hexagonalGrid;
    TileCatalog tileCatalog;
    InputManager inputManager;
    CameraController cameraController;
    GameStateManager gameStateManager;
    EnemySpawnerGestionary ennemySpawner;

    private void Awake()
    {
        cam = Camera.main;
        hexagonalGrid = GetComponent<HexagonalGrid>(); 
        tileCatalog = FindFirstObjectByType<TileCatalog>();
        inputManager = FindFirstObjectByType<InputManager>();
        cameraController = FindFirstObjectByType<CameraController>();
        gameStateManager = FindFirstObjectByType<GameStateManager>();
        ennemySpawner = FindFirstObjectByType<EnemySpawnerGestionary>();
    }

    private void Start()
    {
        // Create initial tile
        Tool.ResetTool();
        tmpHDV_GO = Instantiate(tileCatalog.hdvTile.tilePrefabs, Vector3.zero, Quaternion.identity);
        tmpHDV_GO.transform.Bump(1.1f);        
    }

    GameObject tmpHDV_GO;
    public void OnStart()
    {
        Hexagone firstHex = new Hexagone(Vector3.zero, Vector3.zero, null, tmpHDV_GO, tileCatalog.hdvTile);

        hexagonalGrid.hexagones.Add(firstHex);
        hexagonalGrid.hexagoneTiles.Add(0);
        hexagonalGrid.CreateHexPos(firstHex);

        StartCoroutine(Tool.Delay(() => FillHandTile(tileToPlaceCount), 0.001f));

        LifeTileComponent lifeTileComponent = hexagonalGrid.hexagones[0].hexGO.GetComponent<LifeTileComponent>();

        lifeTileComponent.indexInGrid = 0;
        lifeTileComponent.onDeath += hexagonalGrid.RemoveHex;
        lifeTileComponent.onDeath += i => gameStateManager.ChangePhaseToLoose();
    }

    private void Update()
    {
        if (tilesToPlaceVisual.Count > 0 && gameStateManager.currentPhase == GamePhase.Building && gameStateManager.gameState == GameState.Gameplay)
        {
            GetCurrentTileSelected();
            if (currentPlacementSelectedIndex != -1)
            {
                if(inputManager.rightMouseUpInput && !cameraController.isRotating) RotateCurrentTile();
                if(inputManager.leftMouseUpInput && !cameraController.isMoving) CreateNewTile();
            }

            RotateTilesToPlaceVisual();
        }
    }


    void RotateCurrentTile()
    {
        tileToPlaceRotation += 60;
        nextTileToPlaceVisual.transform.DOLocalRotate(new Vector3(0, tileToPlaceRotation, 0), .2f);
    }
    void CreateNewTile()
    {
        AudioManager.instance.PlayClipAt(tilePlaceClips.ToList().GetRandom(), 0, Vector3.zero);

        // Change the name of the tile
        nextTileToPlaceVisual.name = tilesToPlace[0].tilePrefabs.name;

        // Create the tile in the position selected
        hexagonalGrid.CreateNewTile(currentPlacementSelectedIndex, nextTileToPlaceVisual, tilesToPlace[0]);
        nextTileToPlaceVisual = null;

        // Reset the next tile rotation
        float[] tmp = { 0, 60, 120, 180, 240, 300 };
        tileToPlaceRotation = tmp.ToList().GetRandom();

        tilesToPlace.Remove(tilesToPlace[0]);
        tilesToPlaceVisual[0].transform.DOKill();
        Destroy(tilesToPlaceVisual[0].gameObject);
        tilesToPlaceVisual.Remove(tilesToPlaceVisual[0]);

        if (tilesToPlace.Count > 0) PlaceTilesVisual();
        else
        {
            hexagonalGrid.SetActivePlacementHex(false);
            gameStateManager.ChangePhaseToFight();
        }
    }

    void GetCurrentTileSelected()
    {
        Ray ray = cam.ScreenPointToRay(inputManager.mousePosInput);
        float entry;

        if (plane.Raycast(ray, out entry))
        {           
            int tmpPlacementIndex = GetClosestPlacement(ray.GetPoint(entry));
            if (currentPlacementSelectedIndex != tmpPlacementIndex)
            {
                if (tmpPlacementIndex != -1)
                {
                    currentPlacementSelectedIndex = tmpPlacementIndex;

                    if (nextTileToPlaceVisual == null)
                    {
                        nextTileToPlaceVisual = Instantiate(tilesToPlace[0].tilePrefabs, hexagonalGrid.hexagones[currentPlacementSelectedIndex].axialPos, Quaternion.Euler(0, tileToPlaceRotation, 0));
                        nextTileToPlaceVisual.name = "Ghost Tile";

                        if (nextTileToPlaceVisual.TryGetComponent<Defenses>(out Defenses defense))
                        {
                            defense.detectionRangeVisual.SetActive(true);
                        }
                    }
                    else
                    {
                        nextTileToPlaceVisual.transform.position = hexagonalGrid.hexagones[currentPlacementSelectedIndex].axialPos;
                        AudioManager.instance.PlayClipAt(tileMoveClips.ToList().GetRandom(), 0, Vector3.zero);
                    }

                    if (!nextTileToPlaceVisual.activeSelf) nextTileToPlaceVisual.SetActive(true);
                }
                else
                {
                    currentPlacementSelectedIndex = -1;
                    if(nextTileToPlaceVisual != null) nextTileToPlaceVisual.SetActive(false);
                }
            }
        }
    }
    int GetClosestPlacement(Vector3 currentPos)
    {
        int currentHexIndex = -1;
        float minDist = 999;

        for (int i = 0; i < hexagonalGrid.placementTiles.Count; i++)
        {
            float dist = (currentPos - hexagonalGrid.hexagones[hexagonalGrid.placementTiles[i]].axialPos).sqrMagnitude;

            if (dist < minDist)
            {
                currentHexIndex = hexagonalGrid.placementTiles[i];
                minDist = dist;
            }
        }

        if (minDist >= 1.2f) currentHexIndex = -1;
        return currentHexIndex;
    }

    void FillHandTile(int tileCount)
    {
        print("hehe");
        ennemySpawner.PrepareNextWave();

        var tiles = Tool.ShuffleHand(tileCatalog.tilesInInventory, tileCount);
        print(tileCount);
        foreach (var tile in tiles)
        {
            print(tile);
            tilesToPlace.Add(tile);
            GameObject currentTileVisual = Instantiate(tile.tilePrefabs, tilesVisualContent);
            currentTileVisual.transform.localPosition = new Vector3(0, -10, 0);
            tilesToPlaceVisual.Add(currentTileVisual.transform);
        }

        PlaceTilesVisual();
    }
    void PlaceTilesVisual()
    {
        for (int i = 0; i < tilesToPlaceVisual.Count; i++)
        {
            if (tilesToPlaceVisual[i] != null)
            {
                if (i == 0)
                {
                    tilesToPlaceVisual[i].gameObject.GetComponent<Tile>().SetActiveVisual(true);
                    tilesToPlaceVisual[i].transform.DOScale(1.15f, .2f);
                }
                else tilesToPlaceVisual[i].gameObject.GetComponent<Tile>().SetActiveVisual(false);

                tilesToPlaceVisual[i].transform.DOLocalMove(new Vector3(0, tilesVisualSpacing * i, 0), .3f);
            }
        }
    }

    public void SetPhaseToBuild()
    {
        // Set new count of tile to place

        hexagonalGrid.ResetPlacementInGrid();

        hexagonalGrid.SetActivePlacementHex(true);

        ennemySpawner.PrepareNextWave();
        FillHandTile(tileToPlaceCount);
    }

    void RotateTilesToPlaceVisual()
    {
        foreach (Transform current in tilesToPlaceVisual)
        {
            if(current != null)
                current.localRotation = Quaternion.Lerp(current.localRotation, Quaternion.Inverse(cameraController.newRotation), Time.deltaTime * cameraController.movementTime);
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnBuild += SetPhaseToBuild;
    }
    private void OnDisable()
    {
        GameStateManager.OnBuild -= SetPhaseToBuild;
    }
}