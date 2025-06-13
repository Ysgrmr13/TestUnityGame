using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int enemyCount = 3;
    public float gameTime = 0f;
    
    [Header("References")]
    public Transform[] enemySpawnPoints;
    public GameObject enemyPrefab;
    public PlayerController player;
    
    private List<Enemy> activeEnemies = new List<Enemy>();
    private SaveSystem saveSystem;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGame()
    {
        saveSystem = new SaveSystem();
        LoadGame();
        SpawnEnemies();
    }
    
    private void Update()
    {
        gameTime += Time.deltaTime;
    }
    
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            activeEnemies.Add(enemy);
            enemy.OnDeath += OnEnemyDeath;
        }
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        if (enemySpawnPoints.Length > 0)
        {
            return enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].position;
        }
        return new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
    }
    
    private void OnEnemyDeath(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        enemy.OnDeath -= OnEnemyDeath;
    }
    
    public void SaveGame()
    {
        if (player == null)
        {
            Debug.LogError("Player object not found! Make sure your player has the 'Player' tag and is assigned in GameManager.");
            return;
        }
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on player object!");
            return;
        }
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem.Instance is null!");
            return;
        }
        GameData data = new GameData
        {
            playerPosition = player.transform.position,
            playerHealth = playerHealth.CurrentHealth,
            inventoryData = InventorySystem.Instance.GetSaveData(),
            gameTime = gameTime
        };
        
        saveSystem.SaveGame(data);
    }
    
    public void LoadGame()
    {
        GameData data = saveSystem.LoadGame();
        if (data != null)
        {
            if (player != null)
            {
                player.transform.position = data.playerPosition;
                player.GetComponent<PlayerHealth>().SetHealth(data.playerHealth);
            }
            
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.LoadSaveData(data.inventoryData);
            }
            
            gameTime = data.gameTime;
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveGame();
    }
}