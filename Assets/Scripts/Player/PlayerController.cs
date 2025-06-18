using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerShooting))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed = 5f;
	public FixedJoystick joystick;

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

		rb.interpolation = RigidbodyInterpolation2D.Interpolate; 
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
		rb.freezeRotation = true; 
	}

	private void Update()
	{
		HandleInput();
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
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
		Vector2 rawInput = joystick != null ? new Vector2(joystick.Horizontal, joystick.Vertical) : Vector2.zero;

		if (rawInput.magnitude < 0.1f)
		{
			moveInput = Vector2.zero;
		}
		else
		{
			moveInput = rawInput.normalized;
		}

		if (moveInput.magnitude > 0.1f)
		{
			lastMoveDirection = moveInput;
		}
	}

	private void Move()
	{
		Vector2 targetPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
		rb.MovePosition(targetPosition);

		if (Mathf.Abs(moveInput.x) > 0.1f)
		{
			Vector3 scale = transform.localScale;
			scale.x = moveInput.x > 0 ? 1 : -1;
			transform.localScale = scale;
		}
	}

	public void Shoot()
	{
		shooting.Shoot(lastMoveDirection);
	}

	public Vector2 GetLastMoveDirection()
	{
		return lastMoveDirection;
	}
}