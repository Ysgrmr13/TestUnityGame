using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log("Player died!");
        // Здесь можно добавить экран смерти или перезапуск уровня
    }
    
}