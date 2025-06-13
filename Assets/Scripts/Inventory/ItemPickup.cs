// ВЫРЕЗАТЬ ЭТОТ ФРАГМЕНТ ИЗ ITEM.CS
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
	[Header("Item Pickup")]
	public Item item;
	public int quantity = 1;

	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null && item != null)
		{
			spriteRenderer.sprite = item.icon;
		}
	}

	public Item GetItem()
	{
		return item;
	}

	public void PickUp()
	{
		// Эффект подбора (опционально)
		Destroy(gameObject);
	}
}