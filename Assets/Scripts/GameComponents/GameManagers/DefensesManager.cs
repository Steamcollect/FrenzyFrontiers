using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensesManager : MonoBehaviour
{
    public List<Defenses> defences = new List<Defenses>();

    GameStateManager gameStateManager;

    private void Awake()
    {
        gameStateManager = FindFirstObjectByType<GameStateManager>();
    }

    private void Update()
    {
        if (gameStateManager.currentPhase != GamePhase.Fighting && gameStateManager.gameState != GameState.Gameplay) return;
        if (gameStateManager.gameEnd) return;

        defences.RemoveAll(x => x == null);

        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i].target == null) defences[i].target = defences[i].FindTarget();
            else
            {
                if (Vector3.Distance(defences[i].transform.position, defences[i].target.position) > defences[i]._stats.attackRange * 1.1f)
                {
                    defences[i].target = null;
                    return;
                }

                defences[i].SetVisualToFocusTarget();

                if (defences[i].canAttack)
                {
                    defences[i].Attack(defences[i].target.position);
                    StartCoroutine(defences[i].AttackCooldown());
                }
            }
        }
    }
}