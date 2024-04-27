using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
public class SCO_TileData : ScriptableObject
{
    [Header("Prefabs")]
    public GameObject tilePrefabs;
    [HideInInspector] public SynergyTile[] synergysPossibles;

    [Header("Type Construction")]
    public TypeTile typeTile;
    [HideInInspector] public TypeTile[] typeNeedToBuild;

    [Header("Characteristic shuffle")]
    public EnemyType[] tileEnemyAccess;
    public float powerTile;
    [Range(0f,100f)] public float raretyCardSpawn;

}

[System.Serializable]
public enum TypeTile
{
    Building,
    Tower,
}

[System.Serializable]
public class SynergyTile
{
    public SCO_TileData tileReferences;

    public float attackDamageGiven;
    public float attackSpeedGiven;
    public float healthGiven;
}