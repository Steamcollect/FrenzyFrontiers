using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LifeData",menuName = "ScriptableObjects/LifeData")]
public class SCO_LifeData : ScriptableObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxShield;

    public float MaxHealth{ get { return maxHealth; } }
    public float MaxShield { get {  return maxShield; } }
}
