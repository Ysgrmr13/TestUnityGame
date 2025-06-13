using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI Elements")]
    public Button shootButton;
    public TextMeshProUGUI ammoText;
    public HealthBar playerHealthBar;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        SetupUI();
    }
    
    private void SetupUI()
    {
        shootButton.onClick.AddListener(OnShootButtonPressed);
        
        // Активируем полосу здоровья
        if (playerHealthBar != null)
        {
            playerHealthBar.gameObject.SetActive(true);
        }
        
        // Подписываемся на события здоровья игрока
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
        }
    }
    
    private void OnShootButtonPressed()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.Shoot();
        }
    }
    
    
    
    public void UpdateAmmoDisplay(int ammo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {ammo}";
        }
    }
    
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }
}