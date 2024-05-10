using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public string targetTag;

    public int damage;

    public Vector3 initialDirection;
    public float initialVelocity;

    public abstract void Move();

    public abstract void OnCollision(GameObject hit);

    private void OnTriggerEnter(Collider other)
    {
        //print("Enter");

        if (other.CompareTag(targetTag))
        {
        //print("Target enter");
            OnCollision(other.gameObject);
        }
    }
}