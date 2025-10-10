using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; //Singleton for kill registry

    [Header("Spawn Settings")] 
    [SerializeField] private GameObject[] enemyPrefabs; 
 
    [SerializeField] private int[] killThresholds = { 40, 60, 80, 100 };
  
    private List<Enemy> enemyPool = new List<Enemy>();
    [SerializeField] private int poolSize = 150; // Pool initial size
    [SerializeField] private int maxActiveEnemies = 90; // Max enemies at same time
    
    [SerializeField] private int enemiesPerSpawn = 2;
    [SerializeField] private int killsToIncreaseSpawn = 75;
    
    [SerializeField] private float spawnInterval = 0.4f; // Spawn interval in seconds
    [SerializeField] private float spawnRadius = 8f; 

    [SerializeField] private KillCounterUI killUI;
    [SerializeField] private int killCount = 0;
    public int KillCount => killCount; // Public Kill count

    private List<Enemy> activeEnemies = new List<Enemy>(); // Active enemies list
    private Transform player;
    private float timer;

    void Awake()
    {
        //Singleton definition
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize pool
        Player playerRef = FindAnyObjectByType<Player>();
        
        if (playerRef != null) player = playerRef.transform;

        foreach (GameObject prefab in enemyPrefabs)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false); // Start inactive
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null) enemyPool.Add(enemy);
            }
        }
    }
    
    public void UpdateEnemyManager()
    {
        // Update enemies and control spawn
        UpdateEnemies();
        HandleSpawning();
    }
    
    // Update all active enemies and remove dead ones
    public void UpdateEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            // Get current enemy
            Enemy enemy = activeEnemies[i];

            if (enemy != null && !enemy.isDead)
            {
                enemy.UpdateEnemy();
            }
            else if (enemy != null && enemy.isDead)
            {
                activeEnemies.RemoveAt(i);
                RegisterKill();
            }
            else
            {
                activeEnemies.RemoveAt(i);
            }
        }
    }
    
    // Controls time between spawns
    public void HandleSpawning()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (activeEnemies.Count >= maxActiveEnemies)
            {
                timer = spawnInterval;
                return;
            }

            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (activeEnemies.Count >= maxActiveEnemies) break;
                SpawnEnemy();
            }

            timer = spawnInterval;
        }
    }

   // Spawn new enemy
    public void SpawnEnemy()
    {
        // Use dead enemy
        if (player == null) return;
        
        // Calculate spawn position around player
        Vector2 spawnPos = (Vector2)player.position + UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;

        // Choose prefab 
        GameObject chosenPrefab = ChooseEnemyPrefab();
        
        // 1 Search for chosen enemy type
        foreach (Enemy enemy in enemyPool)
        {
            if (!enemy.gameObject.activeInHierarchy && enemy.gameObject.name.Contains(chosenPrefab.name))
            {
                enemy.transform.position = spawnPos;
                enemy.Initialize(player);
                enemy.gameObject.SetActive(true);
                activeEnemies.Add(enemy);
                return; // If it was possible to reuse dead enemy, returns
            }
        }
        
        // 2 If no dead enemy, Instantiate a new one
        GameObject enemyObj = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);
        Enemy newEnemy = enemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(player);
            activeEnemies.Add(newEnemy);
            enemyPool.Add(newEnemy); // Add to pool for reuse
        }
        else
        {
            Debug.LogWarning("[EnemyManager] Spawned object does not have an Enemy component!");
        }
    }
    
    // Choose prefab to spawn based on kills
    GameObject ChooseEnemyPrefab()
    {
        int unlocked = Mathf.Min(5, enemyPrefabs.Length);

        for (int i = 0; i < killThresholds.Length; i++)
        {
            if (killCount < killThresholds[i])
            {
                unlocked = i + 1;
                break;
            }
        }
        
        if (killCount >= killThresholds[killThresholds.Length - 1])
            unlocked = enemyPrefabs.Length;

        return enemyPrefabs[UnityEngine.Random.Range(0, unlocked)];
    }
    
    // Kill counter and difficulty scaler
    public void RegisterKill()
    {
        killCount++;
        Debug.Log("[EnemyManager] Total Kills = " + killCount);

        // Update HUD
        if (killUI != null)
        {
            killUI.UpdateKillCount(killCount);
        }
        
        // Decrease spawn interval but not below 0.5 seconds
        if (killCount % 80 == 0 && spawnInterval > 0.5f)
        {
            spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.2f);
            Debug.Log("[EnemyManager] Spawn interval accelerated: " + spawnInterval);
        }
        
        // Increase enemies per spawn every X kills
        if (killCount % killsToIncreaseSpawn == 0)
        {
            enemiesPerSpawn++;
            Debug.Log("[EnemyManager] Enemies per spawn increased: " + enemiesPerSpawn);
        }
    }
}