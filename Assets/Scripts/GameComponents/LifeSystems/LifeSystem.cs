using System;
using UnityEngine;

public abstract class LifeSystem : MonoBehaviour, IDamage
{
    public Action onLifeUpdated;
    protected float currentHealth;
    protected float currentShield;
    [SerializeField] protected SCO_LifeData dataLife;

    [SerializeField] ParticleSystem hitParticle;

    protected virtual void Awake()
    {
        if (dataLife == null) throw new ArgumentNullException("LifeData not assign");
        else OverrideLife();
    }

    public virtual void GetDamage(float damageValue)
    {
        if (hitParticle != null) hitParticle.Play();
        if (currentShield > 0) ApplyOnShield(ref damageValue);
        if (damageValue > 0) ApplyOnHealth(ref damageValue);
        onLifeUpdated?.Invoke();
    }

    protected virtual void ApplyOnShield(ref float value)
    {
        float bufferDamage = value - currentShield;
        currentShield = Mathf.Clamp(currentShield - value, 0,dataLife.MaxShield);
        value = bufferDamage;
    }

    protected virtual void ApplyOnHealth(ref float value)
    {
        currentHealth= Mathf.Clamp(currentHealth - value, 0, dataLife.MaxHealth);
    }

    protected virtual void CheckDeath()
    {
        //for override
    }

    //Interface Damage + Poo
    public virtual void TakeDamage(float amountDamage)
    {
        GetDamage(amountDamage);
    }

    public float GetLife()
    {
        return currentHealth;
    }

    public void ChangeDataLife(SCO_LifeData lifeData)
    {
        dataLife = lifeData;
        OverrideLife();
    }
    protected void OverrideLife()
    {
        currentHealth = dataLife.MaxHealth;
        currentShield = dataLife.MaxShield;
    }
}

public interface IDamage
{
    void TakeDamage(float amountDamage);
}