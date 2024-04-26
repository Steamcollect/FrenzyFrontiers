using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Defenses_Mortar : Defenses
{
    [Header("Mortar references")]
    public float initialVelocity;
    public float angle;

    public Animator anim;

    public Transform attackPoint;
    public GameObject bulletPrefabs;

    public Transform rotatePoint;
    public float rotateTime;

    Vector3 lookDir;

    public override void Attack(Vector3 targetPos)
    {
        //Attack
        anim.SetTrigger("Attack");

        Projectile_Bombe currentProjectile = Instantiate(bulletPrefabs, attackPoint.position, attackPoint.rotation).GetComponent<Projectile_Bombe>();
        currentProjectile.initialVelocity = bulletVelocity;
        currentProjectile.initialAngle = angle;
        currentProjectile.initialTargetPos = targetPos;
        currentProjectile.initialPosition = attackPoint.position;
        currentProjectile.targetTag = "Enemy";
        currentProjectile.damage = attackDamage;

        currentProjectile.Move();
    }

    public override void SetVisualToFocusTarget()
    {
        lookDir = target.position - transform.position;

        float finalAngle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, finalAngle, rotateTime);
        rotatePoint.rotation = Quaternion.Euler(0, angle, 0);
    }
}