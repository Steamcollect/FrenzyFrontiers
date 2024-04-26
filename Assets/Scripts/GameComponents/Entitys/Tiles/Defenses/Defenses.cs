using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Defenses : MonoBehaviour
{
    public int attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float bulletVelocity;

    public bool canAttack = true;

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

        Collider[] collidHit = Physics.OverlapSphere(transform.position, attackRange);

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
            StopAllCoroutines();
            StartCoroutine(AttackCooldown());
            return hit.transform;
        }
    }

    public IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}