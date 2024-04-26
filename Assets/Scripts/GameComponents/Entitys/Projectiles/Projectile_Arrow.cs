using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Arrow : Projectile
{
    [Header("Bombe references")]
    public float explosionRange;

    public Vector3 initialTargetPos;
    public float initialAngle;
    public Vector3 initialPosition;

    float currentRotation;
    public float rotationSpeed;

    Coroutine movementCoroutine;

    public override void Move()
    {
        currentRotation = transform.rotation.x;
        Vector3 direction = initialTargetPos - initialPosition;
        Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);

        Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);

        float height = targetPos.y + targetPos.magnitude / 2f;
        height = Mathf.Max(.01f, height);
        float angle, v0, time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

        StopAllCoroutines();
        movementCoroutine = StartCoroutine(Movement(groundDirection.normalized, v0, angle, time));
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

            float xRot = v0 * (t + Time.deltaTime) * Mathf.Cos(angle);
            float yRot = v0 * (t + Time.deltaTime) * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.LookAt(initialPosition + direction * xRot + Vector3.up * yRot);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    public override void OnCollision(GameObject hit)
    {
        if (hit.CompareTag(targetTag))
        {
            hit.GetComponent<IDamage>().TakeDamage(damage);

            if (hit.GetComponent<LifeSystem>().GetLife() > 0)
            {
                StopCoroutine(movementCoroutine);
                transform.SetParent(hit.transform);
            }
        }
    }
}