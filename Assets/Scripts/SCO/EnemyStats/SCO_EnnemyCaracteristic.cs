using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnnemyCaracteristic",menuName = "ScriptableObjects/EnnemyCaracteristic")]
public class SCO_EnnemyCaracteristic : ScriptableObject
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
