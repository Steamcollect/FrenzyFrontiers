using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defenses_Canon : Defenses
{
    [Header("Canon references")]
    public Transform attackPoint;
    public GameObject bulletPrefabs;

    public Transform rotatePoint;
    public float rotateTime;

    Vector3 lookDir;
    float angle;

    public override void Attack(Vector3 targetPos)
    {
        //Attack
        Projectile currentProjectile = Instantiate(bulletPrefabs, attackPoint.position, attackPoint.rotation).GetComponent<Projectile>();
        currentProjectile.initialVelocity = _stats.bulletVelocity;
        currentProjectile.initialDirection = attackPoint.forward;
        currentProjectile.targetTag = "Enemy";
        currentProjectile.damage = _stats.attackDamage;
        currentProjectile.Move();
    }

    public override void SetVisualToFocusTarget()
    {
        finishRotate = false;

        lookDir = target.position - transform.position;

        float finalAngle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, finalAngle, rotateTime);
        rotatePoint.rotation = Quaternion.Euler(0, angle, 0);

        if (Mathf.Abs(angle - finalAngle) <= 1.5f) finishRotate = true;
    }
}