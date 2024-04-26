using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Bombe : Projectile
{
    [Header("Bombe references")]
    public float explosionRange;

    public Vector3 initialTargetPos;
    public float initialAngle;
    public Vector3 initialPosition;

    public GameObject eplosionEffect;

    public override void Move()
    {
        Vector3 direction = initialTargetPos - initialPosition;
        Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);

        Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);

        float height = targetPos.y + targetPos.magnitude / 2f;
        height = Mathf.Max(.01f, height);
        float angle, v0, time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

        StopAllCoroutines();
        StartCoroutine(Movement(groundDirection.normalized, v0, angle, time));
    }

    float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }

    void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / xt);

        v0 = b / Mathf.Sin(angle);
    }

    IEnumerator Movement(Vector3 direction, float v0, float angle, float time)
    {
        float t = 0;

        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.position = initialPosition + direction * x + Vector3.up * y;

            t += Time.deltaTime;
            yield return null;
        }

        Explode();
    }
    public override void OnCollision(GameObject hit)
    {
        // Do nothing
    }

    void Explode()
    {
        Instantiate(eplosionEffect, transform.position, transform.rotation);

        Collider[] collidHit = Physics.OverlapSphere(transform.position, explosionRange);

        if(collidHit.Length > 0)
        {
            foreach (Collider current in collidHit)
            {
                if (current.CompareTag(targetTag))
                {
                    current.GetComponent<IDamage>().TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject, .01f);
    }
}