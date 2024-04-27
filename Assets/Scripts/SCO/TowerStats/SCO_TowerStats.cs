using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats" ,menuName = "ScriptableObjects/TowerStats")]
public class SCO_TowerStats : ScriptableObject
{
    public int attackDamage;
    public float attackRange;
    public float attackCoolDown;
    public float bulletVelocity;
}