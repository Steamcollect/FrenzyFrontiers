using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if(currentHealth <= 4 && !darkSmokeParticle.isPlaying)
        {
            darkSmokeParticle.Play();
            greySmokePaticle.Stop();
        }
        else if(currentHealth <= 10 && !greySmokePaticle.isPlaying)
        {
            greySmokePaticle.Play();
        }

        transform.Bump(1.05f);

        base.ApplyOnHealth(ref value);
        CheckDeath();
    }
}
