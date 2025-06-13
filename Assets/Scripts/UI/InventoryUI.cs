using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
	public static InventoryUI Instance { get; private set; }

	[Header("UI References")]
	public GameObject inventoryPanel;
	public GameObject slotPrefab;
	public Transform slotsContainer; // Может быть null - создастся автоматически
	public Button inventoryButton; // Кнопка для открытия инвентаря
	public Button closeButton; // Кнопка для закрытия инвентаря
	public KeyCode toggleKey = KeyCode.Tab;

	[Header("UI Settings")]
	public int slotsPerRow = 5;
	public int totalSlots = 20; // Фиксированное количество слотов

	private List<ItemSlotUI> slotUIs = new List<ItemSlotUI>();
	private bool isInventoryOpen = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		// Настройка кнопки инвентаря
		if (inventoryButton != null)
		{
			inventoryButton.onClick.AddListener(ToggleInventory);
		}

		// Настройка кнопки закрытия
		if (closeButton != null)
		{
			closeButton.onClick.AddListener(CloseInventory);
		}

		// Проверяем и создаем контейнер если нужно
		if (slotsContainer == null)
		{
			CreateSlotsContainer();
		}

		// Ждем, пока InventorySystem инициализируется
		if (InventorySystem.Instance != null)
		{
			InitializeInventory();
		}
		else
		{
			Invoke("InitializeInventory", 0.1f);
		}
	}

	private void CreateSlotsContainer()
	{
		if (inventoryPanel == null)
		{
			Debug.LogError("InventoryPanel не назначен!");
			return;
		}

		// Настраиваем основную панель инвентаря
		SetupInventoryPanel();

		// Создаем контейнер
		GameObject containerObj = new GameObject("SlotsContainer");
		containerObj.transform.SetParent(inventoryPanel.transform, false);

		// Добавляем RectTransform
		RectTransform rectTransform = containerObj.AddComponent<RectTransform>();

		// Настраиваем якоря для заполнения родителя
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;

		// Добавляем Grid Layout Group
		GridLayoutGroup gridLayout = containerObj.AddComponent<GridLayoutGroup>();
		gridLayout.padding = new RectOffset(20, 20, 20, 20);
		gridLayout.cellSize = new Vector2(80, 80);
		gridLayout.spacing = new Vector2(5, 5);
		gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
		gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
		gridLayout.childAlignment = TextAnchor.UpperCenter;
		gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayout.constraintCount = slotsPerRow;

		slotsContainer = containerObj.transform;
	}

	private void SetupInventoryPanel()
	{
		RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
		if (panelRect == null) return;

		// Настраиваем якоря для центрирования панели
		panelRect.anchorMin = new Vector2(0.5f, 0.5f);
		panelRect.anchorMax = new Vector2(0.5f, 0.5f);
		panelRect.pivot = new Vector2(0.5f, 0.5f);
		panelRect.anchoredPosition = Vector2.zero;

		// Вычисляем и устанавливаем размер панели
		Vector2 panelSize = CalculatePanelSize();
		panelRect.sizeDelta = panelSize;

		// Добавляем фон если его нет
		Image backgroundImage = inventoryPanel.GetComponent<Image>();
		if (backgroundImage == null)
		{
			backgroundImage = inventoryPanel.AddComponent<Image>();
			backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // Полупрозрачный темный фон
		}
	}

	private Vector2 CalculatePanelSize()
	{
		// Параметры Grid Layout
		Vector2 cellSize = new Vector2(80, 80);
		Vector2 spacing = new Vector2(5, 5);
		RectOffset padding = new RectOffset(20, 20, 20, 20);

		// Вычисляем количество рядов
		int rows = Mathf.CeilToInt((float)totalSlots / slotsPerRow);

		// Вычисляем размер панели
		float panelWidth = (cellSize.x * slotsPerRow) +
						   (spacing.x * (slotsPerRow - 1)) +
						   padding.left + padding.right;

		float panelHeight = (cellSize.y * rows) +
							(spacing.y * (rows - 1)) +
							padding.top + padding.bottom;

		return new Vector2(panelWidth, panelHeight);
	}

	private void InitializeInventory()
	{
		CreateInventorySlots();
		inventoryPanel.SetActive(false);

		// Подписываемся на изменения инвентаря
		if (InventorySystem.Instance != null)
		{
			InventorySystem.Instance.OnInventoryChanged += UpdateInventoryUI;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			ToggleInventory();
		}
	}

	private void CreateInventorySlots()
	{
		if (slotsContainer == null)
		{
			Debug.LogError("SlotsContainer все еще null!");
			return;
		}

		if (slotPrefab == null)
		{
			Debug.LogError("SlotPrefab не назначен!");
			return;
		}

		// Очищаем существующие слоты
		foreach (Transform child in slotsContainer)
		{
			Destroy(child.gameObject);
		}
		slotUIs.Clear();

		// Создаем фиксированное количество слотов
		for (int i = 0; i < totalSlots; i++)
		{
			GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
			ItemSlotUI slotUI = slotObj.GetComponent<ItemSlotUI>();

			if (slotUI == null)
			{
				Debug.LogError("ItemSlotUI компонент не найден на префабе слота!");
				continue;
			}

			slotUIs.Add(slotUI);
		}
	}

	public void ToggleInventory()
	{
		isInventoryOpen = !isInventoryOpen;
		inventoryPanel.SetActive(isInventoryOpen);

		if (isInventoryOpen)
		{
			UpdateInventoryUI();
		}
	}

	public void CloseInventory()
	{
		isInventoryOpen = false;
		inventoryPanel.SetActive(false);
	}

	public void OpenInventory()
	{
		isInventoryOpen = true;
		inventoryPanel.SetActive(true);
		UpdateInventoryUI();
	}

	private void UpdateInventoryUI()
	{
		if (InventorySystem.Instance == null) return;

		List<ItemStack> items = InventorySystem.Instance.GetItems();

		// Обновляем все слоты
		for (int i = 0; i < slotUIs.Count; i++)
		{
			if (i < items.Count && items[i] != null)
			{
				slotUIs[i].SetItem(items[i], i);
			}
			else
			{
				slotUIs[i].SetItem(null, i);
			}
		}
	}

	// Публичный метод для обновления размера панели (если нужно изменить количество слотов)
	public void UpdatePanelSize()
	{
		if (inventoryPanel == null) return;

		Vector2 newSize = CalculatePanelSize();
		RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
		if (panelRect != null)
		{
			panelRect.sizeDelta = newSize;
		}
	}

	private void OnDestroy()
	{
		// Отписываемся от событий
		if (InventorySystem.Instance != null)
		{
			InventorySystem.Instance.OnInventoryChanged -= UpdateInventoryUI;
		}

		// Отписываемся от кнопок
		if (inventoryButton != null)
		{
			inventoryButton.onClick.RemoveListener(ToggleInventory);
		}

		if (closeButton != null)
		{
			closeButton.onClick.RemoveListener(CloseInventory);
		}
	}
}