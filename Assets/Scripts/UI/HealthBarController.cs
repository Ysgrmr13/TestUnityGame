using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public HealthBar healthBar;
    public Vector3 offset = new Vector3(0, 2f, 0);
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        // Инициализация компонентов
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogError("Canvas не найден!");
            return;
        }

        if (healthBar == null)
        {
            Debug.LogError("HealthBar компонент не назначен!");
            return;
        }

        // Активируем все уровни иерархии
        canvas.gameObject.SetActive(true);
        gameObject.SetActive(true);
        healthBar.gameObject.SetActive(true);
        
        if (playerHealth != null)
        {
            healthBar.UpdateHealth(playerHealth.CurrentHealth, playerHealth.maxHealth);
            playerHealth.OnHealthChanged += OnHealthChanged;
            Debug.Log("HealthBar инициализирован и активирован");
        }
        else
        {
            Debug.LogError("PlayerHealth не назначен!");
        }
    }

    void LateUpdate()
    {
        if (playerHealth != null && canvas != null)
        {
            // Конвертируем позицию игрока в экранные координаты
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerHealth.transform.position + offset);

            // Конвертируем экранные координаты в локальные координаты Canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint,
                canvas.worldCamera,
                out Vector2 localPoint))
            {
                rectTransform.localPosition = localPoint;
            }
        }
    }

    void OnHealthChanged(float current, float max)
    {
        healthBar.UpdateHealth(current, max);
    }
}