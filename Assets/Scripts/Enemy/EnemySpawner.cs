using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int maxEnemies = 3;
    public float spawnDelay = 5f;
    public float minDistanceFromPlayer = 5f;
    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Transform player;
    
    private void Start()
    {
        player = FindObjectOfType<PlayerController>()?.transform;
        SpawnInitialEnemies();
    }
    
    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }
    
    private void SpawnEnemy()
    {
        if (spawnedEnemies.Count >= maxEnemies) return;
        
        Vector3 spawnPosition = GetValidSpawnPosition();
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);
        
        // Подписываемся на смерть врага
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.OnDeath += OnEnemyDeath;
        }
    }
    
    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPos;
        int attempts = 0;
        
        do
        {
            if (spawnPoints.Length > 0)
            {
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            }
            else
            {
                spawnPos = new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    0
                );
            }
            attempts++;
        }
        while (player != null && 
               Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer && 
               attempts < 10);
        
        return spawnPos;
    }
    
    private void OnEnemyDeath(Enemy enemy)
    {
        GameObject enemyObj = enemy.gameObject;
        if (spawnedEnemies.Contains(enemyObj))
        {
            spawnedEnemies.Remove(enemyObj);
        }
        
        // Респавн врага через время
        Invoke(nameof(SpawnEnemy), spawnDelay);
    }
}