using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bullet : Projectile
{
    [Header("Canon references")]
    public GameObject impactEffect;

    float time = 2;

    public override void Move()
    {
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        float t = 0;

        while (t < time)
        {
            transform.position = transform.position + initialDirection * initialVelocity * Time.deltaTime;

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject, .01f);
    }

    public override void OnCollision(GameObject hit)
    {
        hit.GetComponent<IDamage>().TakeDamage(damage);
        //print(hit.name + " was hited");
        if (impactEffect != null) Instantiate(impactEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, .01f);
    }
}