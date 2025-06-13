using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
	[Header("UI References")]
	public Image itemIcon;
	public TextMeshProUGUI quantityText;
	public Button slotButton;
	public Button deleteButton;
	public Image slotBackground;

	[Header("Visual Settings")]
	public Color normalColor = Color.white;
	public Color selectedColor = Color.yellow;
	public Color emptySlotColor = new Color(0.8f, 0.8f, 0.8f, 1f);

	private ItemStack itemStack;
	private int slotIndex;
	private bool isSelected = false;

	private void Start()
	{
		// Получаем компоненты автоматически, если не назначены
		if (slotButton == null)
			slotButton = GetComponent<Button>();
		if (slotBackground == null)
			slotBackground = GetComponent<Image>();
		if (itemIcon == null)
			itemIcon = transform.Find("ItemIcon")?.GetComponent<Image>();
		if (quantityText == null)
			quantityText = transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
		if (deleteButton == null)
			deleteButton = transform.Find("DeleteButton")?.GetComponent<Button>();

		// Настраиваем события
		if (slotButton != null)
			slotButton.onClick.AddListener(OnSlotClicked);

		if (deleteButton != null)
		{
			deleteButton.onClick.AddListener(OnDeleteClicked);
			deleteButton.gameObject.SetActive(false);
		}

		// Устанавливаем начальный цвет
		if (slotBackground != null)
			slotBackground.color = emptySlotColor;
	}

	public void SetItem(ItemStack stack, int index)
	{
		itemStack = stack;
		slotIndex = index;

		if (stack != null && stack.item != null)
		{
			// Отображаем иконку предмета
			if (itemIcon != null)
			{
				itemIcon.sprite = stack.item.icon;
				itemIcon.color = Color.white;
			}

			// Показываем количество
			if (quantityText != null)
			{
				if (stack.quantity > 1)
				{
					quantityText.text = stack.quantity.ToString();
					quantityText.gameObject.SetActive(true);
				}
				else
				{
					quantityText.gameObject.SetActive(false);
				}
			}

			// Меняем цвет фона для заполненного слота
			if (slotBackground != null)
				slotBackground.color = normalColor;
		}
		else
		{
			// Очищаем слот
			if (itemIcon != null)
			{
				itemIcon.sprite = null;
				itemIcon.color = Color.clear;
			}

			if (quantityText != null)
				quantityText.gameObject.SetActive(false);

			if (deleteButton != null)
				deleteButton.gameObject.SetActive(false);

			// Цвет для пустого слота
			if (slotBackground != null)
				slotBackground.color = emptySlotColor;

			SetSelected(false);
		}
	}

	private void OnSlotClicked()
	{
		if (itemStack != null && itemStack.item != null)
		{
			SetSelected(!isSelected);

			if (isSelected)
			{
				var allSlots = FindObjectsOfType<ItemSlotUI>();
				foreach (var slot in allSlots)
				{
					if (slot != this)
						slot.SetSelected(false);
				}
			}
		}
	}

	public void SetSelected(bool selected)
	{
		isSelected = selected;

		if (slotBackground != null && itemStack != null)
		{
			slotBackground.color = isSelected ? selectedColor : normalColor;
		}

		if (deleteButton != null)
		{
			deleteButton.gameObject.SetActive(isSelected && itemStack != null);
		}
	}

	private void OnDeleteClicked()
	{
		if (itemStack != null && InventorySystem.Instance != null)
		{
			InventorySystem.Instance.RemoveItemAt(slotIndex);
			SetSelected(false);
		}
	}
}