using UnityEngine;
using System;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 10f;
    protected float currentHealth;
    public bool isDead = false;
    
    public event Action< float, float > OnHealthChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(float damage)
    {
        if(isDead) return;
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }

    protected abstract void Die();
}
