using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public float playerHealth;
    public List<ItemSaveData> inventoryData;
    public float gameTime;
}

[System.Serializable]
public class ItemSaveData
{
    public string itemId;
    public int quantity;
}

public class SaveSystem
{
    private string savePath;
    
    public SaveSystem()
    {
        savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }
    
    public void SaveGame(GameData data)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, jsonData);
            Debug.Log("Game saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }
    
    public GameData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string jsonData = File.ReadAllText(savePath);
                GameData data = JsonUtility.FromJson<GameData>(jsonData);
                Debug.Log("Game loaded successfully");
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
        }
        
        return null;
    }
}