using UnityEngine;
using System;
using UnityEditor; // <-- ДОБАВЛЕНО: для использования Handles в редакторе

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 50f;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float detectionRange = 5f;
    public float attackCooldown = 2f;
    
    [Header("Drops")]
    public GameObject[] itemDrops;
    public float dropChance = 0.7f;
    
    private float currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private Vector3 initialScale; // Для сохранения начального масштаба спрайта
    
    public event Action<Enemy> OnDeath;
    public event Action<float, float> OnHealthChanged;
    
    private void Start()
    {
        currentHealth = maxHealth;
        initialScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        // Важно: Убедитесь, что у вас есть скрипт PlayerController на игроке, иначе будет ошибка
        // player = FindObjectOfType<PlayerController>()?.transform;
        
        // Более надежный способ найти игрока по тегу:
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found! Make sure your player has the 'Player' tag.");
        }
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else
        {
            // Опционально: остановить врага, если игрок вне зоны видимости
            rb.velocity = Vector2.zero; 
        }
    }
    
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        
        // Поворот спрайта
        if (direction.x > 0.01f)
        transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    else if (direction.x < -0.01f)
        transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }
    
    private void AttackPlayer()
    {
        rb.velocity = Vector2.zero;
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            // Важно: Убедитесь, что у игрока есть скрипт PlayerHealth
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacked player for {attackDamage} damage");
            }
        }
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
    
    private void Die()
    {
        DropItems();
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
    
    private void DropItems()
    {
        if (itemDrops.Length > 0 && UnityEngine.Random.value <= dropChance)
        {
            GameObject itemToDrop = itemDrops[UnityEngine.Random.Range(0, itemDrops.Length)];
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Урон от пуль
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Здесь можно получить урон из самой пули, если у нее есть такой компонент
            // Например: float damage = collision.gameObject.GetComponent<Bullet>().damage;
            TakeDamage(25f); // Пока используем фиксированное значение
            Destroy(collision.gameObject);
        }
    }
    
    // --- ИЗМЕНЕННАЯ ЧАСТЬ ---
    private void OnDrawGizmosSelected()
    {
        // Рисуем радиус обнаружения (желтый)
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.forward, detectionRange);
        
        // Рисуем радиус атаки (красный)
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, attackRange);
    }
}