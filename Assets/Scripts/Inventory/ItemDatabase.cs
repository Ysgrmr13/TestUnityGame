using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase Instance { get; private set; }

	[Header("Items Database")]
	public Item[] allItems;

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

	public Item GetItem(string itemId)
	{
		foreach (var item in allItems)
		{
			if (item.itemId == itemId)
				return item;
		}
		return null;
	}
}
