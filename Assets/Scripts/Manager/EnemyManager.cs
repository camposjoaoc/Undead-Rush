using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; //Singleton simples para registro de kills

    [Header("Spawn Settings")] [SerializeField]
    private GameObject[] enemyPrefabs; //Array de prefabs de inimigos

    [SerializeField] private int[] killThresholds = { 20, 30, 40, 50 }; //Thresholds de kills para dificuldade
    private List<Enemy> enemyPool = new List<Enemy>();
    [SerializeField] private int poolSize = 20; // tamanho inicial do pool

    [SerializeField] private KillCounterUI killUI; //Referência à UI de kills
    [SerializeField] private int enemiesPerSpawn = 1;
    [SerializeField] private int killsToIncreaseSpawn = 50;
    [SerializeField] private float spawnInterval = 1f; //Intervalo de spawn em segundos
    [SerializeField] private float spawnRadius = 8f; //Raio de spawn em unidades
    [SerializeField] private int killCount = 0; //Contador de kills
    public int KillCount => killCount; //Propriedade pública para acessar kills

    private List<Enemy> activeEnemies = new List<Enemy>(); //Lista de inimigos ativos
    private Transform player;
    private float timer;

    void Awake()
    {
        //Define Singleton
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
        /*
        //Busca o player na cena
        Player playerRef = FindAnyObjectByType<Player>();
        if (playerRef != null)
        {
            player = playerRef.transform;
        }
        else
        {
            Debug.LogWarning("[EnemyManager] Player not found in scene!");
        }
        */

        // Inicializa pool
        Player playerRef = FindAnyObjectByType<Player>();
        if (playerRef != null) player = playerRef.transform;

        foreach (GameObject prefab in enemyPrefabs)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false); // começa desativado
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null) enemyPool.Add(enemy);
            }
        }
    }


    public void UpdateEnemyManager()
    {
        //Atualiza inimigos e controla spawn
        UpdateEnemies();
        HandleSpawning();
    }

    /// Atualiza todos os inimigos ativos e remove os mortos.
    public void UpdateEnemies()
    {
        // Percorre a lista de inimigos de trás pra frente para permitir remoção segura
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            // Pega o inimigo atual
            Enemy enemy = activeEnemies[i];

            if (enemy != null && !enemy.isDead) // inimigo está vivo
            {
                enemy.UpdateEnemy(); // atualiza movimento/flip
            }
            else if (enemy != null && enemy.isDead) // inimigo morreu
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

    /// Controla o tempo entre spawns.
    public void HandleSpawning()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnEnemy();
            }

            timer = spawnInterval;
        }
    }

    /// Spawn de um inimigo novo.
    public void SpawnEnemy()
    {
        /*
        if (enemyPrefabs.Length == 0) return;

        //Calcula posição de spawn aleatória em volta do player
        Vector2 spawnPos = (Vector2)player.position + UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;

        GameObject prefab = ChooseEnemyPrefab();
        GameObject enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(player); //Inicializa o inimigo com referência ao player
            activeEnemies.Add(enemy); //Adiciona à lista de inimigos ativos
        }
        else
        {
            Debug.LogWarning("[EnemyManager] Spawned object does not have an Enemy component!");
        }
        */
        // Tenta reaproveitar inimigo morto
        if (player == null) return;

        // Calcula posição de spawn em volta do player
        Vector2 spawnPos = (Vector2)player.position + UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;

        // Escolhe prefab (pode ser goblin, esqueleto, etc)
        GameObject chosenPrefab = ChooseEnemyPrefab();

        // 1. Procura inimigo desativado do tipo escolhido
        foreach (Enemy enemy in enemyPool)
        {
            if (!enemy.gameObject.activeInHierarchy && enemy.gameObject.name.Contains(chosenPrefab.name))
            {
                enemy.transform.position = spawnPos;
                enemy.Initialize(player);
                enemy.gameObject.SetActive(true);
                activeEnemies.Add(enemy);
                return; // reaproveitou → sai
            }
        }

        // 2. Se nenhum disponível no pool, instancia novo
        GameObject enemyObj = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);
        Enemy newEnemy = enemyObj.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            newEnemy.Initialize(player);
            activeEnemies.Add(newEnemy);
            enemyPool.Add(newEnemy); // adiciona ao pool para reaproveitar
        }
        else
        {
            Debug.LogWarning("[EnemyManager] Spawned object does not have an Enemy component!");
        }
    }

    /// Escolhe qual prefab spawnar baseado em kills.
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

        // Se passou de todos os thresholds, libera todos
        if (killCount >= killThresholds[killThresholds.Length - 1])
            unlocked = enemyPrefabs.Length;

        return enemyPrefabs[UnityEngine.Random.Range(0, unlocked)];
    }

    //Conta um kill e pode acelerar dificuldade.
    public void RegisterKill()
    {
        killCount++;
        Debug.Log("[EnemyManager] Total Kills = " + killCount);

        // Atualiza HUD
        if (killUI != null)
        {
            killUI.UpdateKillCount(killCount);
        }

        // Acelera spawn a cada 25 kills
        if (killCount % 25 == 0 && spawnInterval > 0.5f)
        {
            spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.2f);
            Debug.Log("[EnemyManager] Spawn interval acelerated: " + spawnInterval);
        }

        // Aumenta quantidade por ciclo a cada X kills
        if (killCount % killsToIncreaseSpawn == 0)
        {
            enemiesPerSpawn++;
            Debug.Log("[EnemyManager] Enemies per spawn increased: " + enemiesPerSpawn);
        }
    }
}