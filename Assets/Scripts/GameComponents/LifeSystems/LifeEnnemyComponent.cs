using System;

public class LifeEnnemyComponent : LifeSystem
{
    public Action onDeath;

    protected override void CheckDeath()
    {
        if (currentHealth <= 0) 
        {
            ScoreManager.instance.AddScore(5);
            Destroy(gameObject,0.001f);
            onDeath?.Invoke();
        }
    }

    protected override void ApplyOnHealth(ref float value)
    {
        base.ApplyOnHealth(ref value);
        CheckDeath();
    }
}