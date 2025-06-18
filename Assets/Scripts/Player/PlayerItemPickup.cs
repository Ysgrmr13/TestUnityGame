using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerItemPickup : MonoBehaviour
{
	[Header("Pickup Settings")]
	public float pickupRange = 2f;
	public LayerMask itemLayer = -1;

	[Header("Debug")]
	public bool showDebugInfo = true;

	private ItemPickup nearbyItem;
	private Camera playerCamera;

	private void Start()
	{
		playerCamera = Camera.main;
	}

	private void Update()
	{
		CheckForNearbyItems();

		if (nearbyItem != null)
		{
			PickupItem();
		}
	}

	private void CheckForNearbyItems()
	{
		Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, pickupRange, itemLayer);

		ItemPickup closestItem = null;
		float closestDistance = float.MaxValue;


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

		if (closestItem != nearbyItem)
		{
			nearbyItem = closestItem;
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
}