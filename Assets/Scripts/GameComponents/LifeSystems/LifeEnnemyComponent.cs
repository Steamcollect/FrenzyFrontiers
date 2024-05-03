using System;
using UnityEngine;
using System.Linq;

public class LifeEnnemyComponent : LifeSystem
{
    public Action onDeath;

    public AudioClip[] hitSound;
    public AudioClip[] deathSound;

    protected override void CheckDeath()
    {
        if (currentHealth <= 0) 
        {
            AudioManager.instance.PlayClipAt(deathSound.ToList().GetRandom(), 1, transform.position);

            ScoreManager.instance.AddScore(5);
            Destroy(gameObject,0.001f);
            onDeath?.Invoke();
        }
        else AudioManager.instance.PlayClipAt(hitSound.ToList().GetRandom(), 1, transform.position);
    }

    protected override void ApplyOnHealth(ref float value)
    {
        base.ApplyOnHealth(ref value);
        CheckDeath();
    }
}