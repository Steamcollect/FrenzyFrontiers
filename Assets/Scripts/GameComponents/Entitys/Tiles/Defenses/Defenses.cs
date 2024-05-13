using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Defenses : MonoBehaviour
{
    public SCO_TowerStats _stats;

    public bool canAttack = true;
    public bool finishRotate  = true;

    public GameObject detectionRangeVisual;

    [HideInInspector] public Transform target;

    [HideInInspector] public DefensesManager defensesManager;

    private void Awake()
    {
        defensesManager = FindFirstObjectByType<DefensesManager>();
    }

    public abstract void Attack(Vector3 targetPos);
    public abstract void SetVisualToFocusTarget();

    public Transform FindTarget()
    {
        Collider hit = null;
        float minDist = 999;

        finishRotate = true;

        Collider[] collidHit = Physics.OverlapSphere(transform.position, _stats.attackRange);

        foreach (Collider current in collidHit)
        {
            float currentDist = Vector3.Distance(current.transform.position, transform.position);
            if (currentDist < minDist && current.transform.CompareTag("Enemy"))
            {
                hit = current;
                minDist = currentDist;
            }
        }

        if (hit == null) return null;
        else
        {
            return hit.transform;
        }
    }

    public IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(_stats.attackCoolDown);

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _stats.attackRange);
    }
}