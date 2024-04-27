using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCharacteristic",menuName = "ScriptableObjects/EnemyCharacteristic")]
public class SCO_EnemyCharacteristic : ScriptableObject
{
    public float attackDamage;
    public float walkSpeed;
    public float rangeAttack;
    public float cooldownAttack;
}

[System.Serializable]
public enum EnemyType
{
    None,
    Floating,
    Ground,
    Boss,
}
