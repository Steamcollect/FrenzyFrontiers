using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class LifeTileComponent : LifeSystem
{
    public Action<int> onDeath;
    public int indexInGrid;

    [SerializeField] ParticleSystem darkSmokeParticle, greySmokePaticle;

    public AudioClip[] DestroySound;

    protected override void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            AudioManager.instance.PlayClipAt(DestroySound.ToList().GetRandom(), 1, transform.position);

            transform.DOKill();
            onDeath?.Invoke(indexInGrid);
        }
    }

    protected override void ApplyOnHealth(ref float value)
    {        
        if(greySmokePaticle && currentHealth <= 10 && !greySmokePaticle.isPlaying)
        {
            if (darkSmokeParticle && currentHealth <= 4 && !darkSmokeParticle.isPlaying)
            {
                darkSmokeParticle.Play();
                greySmokePaticle.Stop();
            }
            else greySmokePaticle.Play();
        }

        transform.Bump(1.05f);

        base.ApplyOnHealth(ref value);
        CheckDeath();
    }
}
