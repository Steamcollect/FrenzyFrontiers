using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveEnnemy",menuName ="ScriptableObjects/WaveEnnemy")]
public class SCO_EnnemyWave : ScriptableObject
{
    public List<EnemyWave> waveData;
}

[System.Serializable]
public class EnemyWave
{
    public GameObject prefab;
    public EnemyType enemyType;
    public float powerUnit;
    public int waveMin;
}
