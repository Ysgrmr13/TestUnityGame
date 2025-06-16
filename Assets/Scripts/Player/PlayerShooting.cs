using UnityEngine;
using UnityEditor; // <--- 1. ДОБАВЛЕНО: Пространство имен для работы с Handles в редакторе

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
	public string ammoItemId = "ammo1";
	public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float shootRange = 5f;
    
    private int ammunition = 30;
    public int Ammunition => ammunition;

	public void Shoot(Vector2 direction)
	{
		// Проверяем, есть ли вообще инвентарь
		if (InventorySystem.Instance == null)
		{
			Debug.LogError("InventorySystem не найден!");
			return;
		}

		// Пытаемся списать 1 патрон из инвентаря.
		// Если удалось (метод вернул true), значит патроны были, и мы можем стрелять.
		if (InventorySystem.Instance.RemoveItem(ammoItemId, 1))
		{
			// Проверяем наличие врагов в радиусе (если эта логика нужна)
			if (!HasEnemyInRange())
			{
				Debug.Log("No enemies in range!");
				// ВАЖНО: Если врага нет, нужно вернуть патрон обратно, чтобы он не потратился зря!
				InventorySystem.Instance.AddItem(ItemDatabase.Instance.GetItem(ammoItemId), 1);
				return;
			}

			// Создаем пулю
			GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
			Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
			if (bulletRb != null)
			{
				bulletRb.velocity = direction * bulletSpeed;
			}
			Destroy(bullet, 3f);

			// Обновляем UI, но теперь нужно получить актуальное количество патронов из инвентаря.
			// Для этого нам понадобится новый метод в InventorySystem (см. Шаг 3).
			 UIManager.Instance?.UpdateAmmoDisplay(InventorySystem.Instance.GetItemCount(ammoItemId));
		}
		else
		{
			// Если RemoveItem вернул false, значит патронов в инвентаре нет.
			Debug.Log("No ammunition in inventory!");
		}
	}

	private bool HasEnemyInRange()
    {
        // Убедитесь, что у вас есть слой "Enemy" в настройках проекта
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, shootRange, LayerMask.GetMask("Enemy"));
        return enemies.Length > 0;
    }
    
    public void AddAmmo(int amount)
    {
        ammunition += amount;
        UIManager.Instance?.UpdateAmmoDisplay(ammunition);
    }
    
}