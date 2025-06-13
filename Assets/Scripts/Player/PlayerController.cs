using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerShooting))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public FixedJoystick joystick; // Привязка джойстика из Canvas
    
    private Rigidbody2D rb;
    private PlayerHealth health;
    private PlayerShooting shooting;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private Animator animator;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        shooting = GetComponent<PlayerShooting>();
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        HandleInput();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // Устанавливаем параметр Speed для анимации бега
        if (animator != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
        }
    }
    
    private void FixedUpdate()
    {
        Move();
    }
    
    private void HandleInput()
    {
        // Получаем ввод с джойстика, если он назначен
        moveInput = joystick != null ? new Vector2(joystick.Horizontal, joystick.Vertical) : Vector2.zero;
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = moveInput.normalized;
        }
    }
    
    private void Move()
    {
        rb.velocity = moveInput * moveSpeed;
        
        // Поворот спрайта в сторону движения
        if (moveInput.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
    
    public void Shoot()
    {
        shooting.Shoot(lastMoveDirection);
    }
    
    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Подбор предметов
        //ItemPickup item = other.GetComponent<ItemPickup>();
        //if (item != null)
        //{
        //    if (InventorySystem.Instance.AddItem(item.GetItem()))
        //    {
        //        item.PickUp();
        //    }
        //}
    }
}