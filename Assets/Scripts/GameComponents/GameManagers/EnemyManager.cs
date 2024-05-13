using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RSE_EnnemyRequest _rseEnnemyRequestTarget;
    [SerializeField] private HexagonalGrid grid;

    private Queue<EnnemyTemplate> ennemiesSearchTarget = new Queue<EnnemyTemplate>();

    private void OnEnable()
    {
        _rseEnnemyRequestTarget.action += PushRequestTarget;
        GameStateManager.OnPaused += ClearQueue;
        GameStateManager.OnLoose += ClearQueue;
    }
    private void OnDisable()
    {
        _rseEnnemyRequestTarget.action -= PushRequestTarget;
        GameStateManager.OnPaused -= ClearQueue;
        GameStateManager.OnLoose -= ClearQueue;
    }

    private void PushRequestTarget(EnnemyTemplate ennemy) => ennemiesSearchTarget.Enqueue(ennemy);


    private void Update()
    {
        if (ennemiesSearchTarget.Count > 0) FindTargetEnnemy();
    }

    private void ClearQueue() => ennemiesSearchTarget.Clear();

    public void FindTargetEnnemy()
    {
        EnnemyTemplate enemy = ennemiesSearchTarget.Dequeue();
        if (enemy)
        {
            GameObject targetObj = null;
            Vector3 targetPos = Vector3.zero;

            //When merge pass find all, get directy to new list tile

            int indexTile = grid.GetClosestHexagone(UnityEngine.Random.Range(-3.5f,3.6f) * enemy.transform.right + enemy.transform.position);
            if (indexTile == -1) { enemy.SelectTarget(targetPos, targetObj); return; }
            Hexagone hexagoneTile = grid.hexagones[indexTile];

            if (hexagoneTile != null)
            {
                targetObj = hexagoneTile.hexGO;

                targetPos = hexagoneTile.axialPos + (enemy.transform.position - hexagoneTile.axialPos).normalized * grid.horizontalDistance;
            }
            else Debug.LogWarning($"not target for {enemy.name} ,set on Vector3.zero");

            enemy.SelectTarget(targetPos, targetObj);
        }
    }
}