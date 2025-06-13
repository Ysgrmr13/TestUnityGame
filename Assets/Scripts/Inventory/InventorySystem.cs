using UnityEngine;
using System.Collections.Generic;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    
    [Header("Inventory Settings")]
    public int maxSlots = 20;
    
    private List<ItemStack> items = new List<ItemStack>();
    
    public event Action OnInventoryChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool AddItem(Item item, int quantity = 1)
    {
        // Проверяем, можно ли добавить к существующему стаку
        ItemStack existingStack = items.Find(stack => stack.item.itemId == item.itemId);
        
        if (existingStack != null && item.stackable)
        {
            existingStack.quantity += quantity;
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        // Создаем новый стак
        if (items.Count < maxSlots)
        {
            items.Add(new ItemStack(item, quantity));
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        return false; // Инвентарь полон
    }
    
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        ItemStack stack = items.Find(s => s.item.itemId == itemId);
        
        if (stack != null)
        {
            stack.quantity -= quantity;
            
            if (stack.quantity <= 0)
            {
                items.Remove(stack);
            }
            
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        return false;
    }
    
    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
            OnInventoryChanged?.Invoke();
        }
    }
    
    public List<ItemStack> GetItems()
    {
        return new List<ItemStack>(items);
    }
	public List<ItemSaveData> GetSaveData()
    {
        List<ItemSaveData> saveData = new List<ItemSaveData>();
        foreach (var stack in items)
        {
            saveData.Add(new ItemSaveData
            {
                itemId = stack.item.itemId,
                quantity = stack.quantity
            });
        }
        return saveData;
    }
    
    public void LoadSaveData(List<ItemSaveData> saveData)
    {
        items.Clear();
        
        if (saveData != null)
        {
            foreach (var data in saveData)
            {
                Item item = ItemDatabase.Instance.GetItem(data.itemId);
                if (item != null)
                {
                    items.Add(new ItemStack(item, data.quantity));
                }
            }
        }
        
        OnInventoryChanged?.Invoke();
    }
	public int GetItemCount(string itemId)
	{
		ItemStack stack = items.Find(s => s.item.itemId == itemId);
		if (stack != null)
		{
			return stack.quantity;
		}
		return 0; // Возвращаем 0, если такого предмета нет
	}
}

[System.Serializable]
public class ItemStack
{
    public Item item;
    public int quantity;
    
    public ItemStack(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}