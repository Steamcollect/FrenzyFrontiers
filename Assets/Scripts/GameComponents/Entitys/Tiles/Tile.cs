using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LifeTileComponent))]
public class Tile : MonoBehaviour, IDamage
{
    public bool isImmobile;

    public int indexInGrid;

    public GameObject tileVisual;

    [SerializeField] private SCO_TileData tileData;
    private LifeSystem lifeComponent;

    private void Awake()
    {
        lifeComponent = GetComponentInChildren<LifeSystem>();
        if (tileData == null) throw new ArgumentNullException("TileData not assign");
    }

    public void TakeDamage(float amountDamage)
    {
        lifeComponent.GetDamage(amountDamage);
    }

    public void SetActiveVisual(bool isActive)
    {
        tileVisual.SetActive(isActive);
    }
}