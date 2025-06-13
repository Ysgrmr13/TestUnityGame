using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerItemPickup : MonoBehaviour
{
	[Header("Pickup Settings")]
	public float pickupRange = 2f;
	public KeyCode pickupKey = KeyCode.E;
	public LayerMask itemLayer = -1;

	[Header("UI References")]
	public GameObject pickupPrompt;
	public TextMeshProUGUI promptText;

	[Header("Debug")]
	public bool showDebugInfo = true;

	private ItemPickup nearbyItem;
	private Camera playerCamera;

	private void Start()
	{
		playerCamera = Camera.main;
		if (pickupPrompt != null)
			pickupPrompt.SetActive(false);

		// Отладочная информация
		if (showDebugInfo)
		{
			Debug.Log($"PlayerItemPickup инициализирован. Радиус подбора: {pickupRange}, Клавиша: {pickupKey}");
			Debug.Log($"Слой предметов: {itemLayer.value}");
		}
	}

	private void Update()
	{
		CheckForNearbyItems();

		if (Input.GetKeyDown(pickupKey) && nearbyItem != null)
		{
			PickupItem();
		}
	}

	private void CheckForNearbyItems()
	{
		// Используем как Collider2D, так и обычные Collider для совместимости
		Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, pickupRange, itemLayer);
		Collider[] colliders3D = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);

		ItemPickup closestItem = null;
		float closestDistance = float.MaxValue;

		// Проверяем 2D коллайдеры
		foreach (var collider in colliders2D)
		{
			ItemPickup item = collider.GetComponent<ItemPickup>();
			if (item != null && item.item != null)
			{
				float distance = Vector2.Distance(transform.position, item.transform.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestItem = item;
				}
			}
		}

		// Проверяем 3D коллайдеры (если используются)
		foreach (var collider in colliders3D)
		{
			ItemPickup item = collider.GetComponent<ItemPickup>();
			if (item != null && item.item != null)
			{
				float distance = Vector3.Distance(transform.position, item.transform.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestItem = item;
				}
			}
		}

		// Отладочная информация
		if (showDebugInfo && closestItem != nearbyItem)
		{
			if (closestItem != null)
			{
				Debug.Log($"Найден предмет: {closestItem.item.itemName} на расстоянии {closestDistance:F2}");
			}
			else if (nearbyItem != null)
			{
				Debug.Log("Предмет больше не в радиусе");
			}
		}

		// Обновляем UI подсказки
		if (closestItem != nearbyItem)
		{
			nearbyItem = closestItem;
			UpdatePickupPrompt();
		}
	}

	private void UpdatePickupPrompt()
	{
		if (nearbyItem != null)
		{
			if (pickupPrompt != null)
			{
				pickupPrompt.SetActive(true);
				if (promptText != null)
				{
					promptText.text = $"Нажмите {pickupKey} чтобы взять {nearbyItem.item.itemName}";
				}
			}
		}
		else
		{
			if (pickupPrompt != null)
				pickupPrompt.SetActive(false);
		}
	}

	private void PickupItem()
	{
		if (nearbyItem != null && InventorySystem.Instance != null)
		{
			if (showDebugInfo)
			{
				Debug.Log($"Попытка подобрать: {nearbyItem.item.itemName} x{nearbyItem.quantity}");
			}

			bool success = InventorySystem.Instance.AddItem(nearbyItem.item, nearbyItem.quantity);

			if (success)
			{
				if (showDebugInfo)
				{
					Debug.Log($"Предмет {nearbyItem.item.itemName} успешно добавлен в инвентарь!");
				}

				nearbyItem.PickUp();
				{
					 UIManager.Instance?.UpdateAmmoDisplay(InventorySystem.Instance.GetItemCount("ammo1"));
				}
				nearbyItem = null;
				UpdatePickupPrompt();
			}
			else
			{
				if (showDebugInfo)
				{
					Debug.Log("Инвентарь полон!");
				}
				// Здесь можно добавить UI уведомление о том, что инвентарь полон
			}
		}
		else
		{
			if (showDebugInfo)
			{
				Debug.Log("Не удалось подобрать предмет. Проверьте InventorySystem и nearbyItem.");
			}
		}
	}

	// Отладочная визуализация в Scene View
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, pickupRange);

		if (nearbyItem != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, nearbyItem.transform.position);
			Gizmos.DrawWireSphere(nearbyItem.transform.position, 0.2f);
		}
	}

	// Публичные методы для отладки
	public void TestPickup()
	{
		Debug.Log("=== ТЕСТ СИСТЕМЫ ПОДБОРА ===");
		Debug.Log($"InventorySystem есть: {InventorySystem.Instance != null}");
		Debug.Log($"Ближайший предмет: {(nearbyItem != null ? nearbyItem.item.itemName : "НЕТ")}");

		if (nearbyItem != null)
		{
			Debug.Log($"Предмет валиден: {nearbyItem.item != null}");
			Debug.Log($"Расстояние: {Vector3.Distance(transform.position, nearbyItem.transform.position):F2}");
		}

		Collider2D[] allItems = Physics2D.OverlapCircleAll(transform.position, pickupRange * 2f);
		Debug.Log($"Найдено коллайдеров в радиусе {pickupRange * 2f}: {allItems.Length}");

		foreach (var col in allItems)
		{
			ItemPickup pickup = col.GetComponent<ItemPickup>();
			if (pickup != null)
			{
				Debug.Log($"- {pickup.name}: {pickup.item?.itemName ?? "БЕЗ ПРЕДМЕТА"}");
			}
		}
	}
}