using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;
    public Image fillImage;
    public Gradient healthGradient;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Смещение над врагом
    
    private Enemy enemy;
    private RectTransform rectTransform;
    private Canvas canvas;
    
    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        if (enemy != null)
        {
            enemy.OnHealthChanged += UpdateHealthBar;
        }
    }
    
    private void LateUpdate()
    {
        if (enemy != null && canvas != null)
        {
            // Конвертируем позицию врага в экранные координаты
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(enemy.transform.position + offset);
            
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
    
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.value = healthPercentage;
        }
        
        if (fillImage != null)
        {
            fillImage.color = healthGradient.Evaluate(healthPercentage);
        }
    }
}