using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;
    public Image fillImage;
    public Gradient healthGradient;
    
    public void UpdateHealth(float currentHealth, float maxHealth)
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