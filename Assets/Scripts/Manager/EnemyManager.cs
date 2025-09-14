using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; //Singleton simples para registro de kills

    [Header("Spawn Settings")] [SerializeField]
    private GameObject[] enemyPrefabs; //Array de prefabs de inimigos

    [SerializeField] private KillCounterUI killUI;  //Referência à UI de kills
    
    [SerializeField] private float spawnInterval = 2f; //Intervalo de spawn em segundos
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
        //Busca o player na cena
        Player playerRef = FindAnyObjectByType<Player>();
        if (playerRef != null)
        {
            player = playerRef.transform;
        }
        else
        {
            Debug.LogWarning("EnemyManager: Player not found in scene!");
        }
    }


    void Update()
    {
        //Atualiza inimigos e controla spawn
        UpdateEnemies();
        HandleSpawning();
    }

    /// <summary>
    /// Atualiza todos os inimigos ativos e remove os mortos.
    /// </summary>
    public void UpdateEnemies()
    {
        // Percorre a lista de inimigos de trás pra frente para permitir remoção segura
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            // Pega o inimigo atual
            Enemy enemy = activeEnemies[i];

            // Verifica se o inimigo ainda está vivo
            if (enemy != null && enemy.IsAlive())
            {
                enemy.UpdateEnemy(); //Atualiza o movimento/flip pro Enemy
            }
            else if (enemy != null && !enemy.IsAlive())
            {
                // remove da lista e conta kill
                activeEnemies.RemoveAt(i);
                RegisterKill();
            }
            else
            {
                // Inimigo foi destruído de outra forma, remove da lista
                activeEnemies.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Controla o tempo entre spawns.
    /// </summary>
    public void HandleSpawning()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    /// <summary>
    /// Spawn de um inimigo novo.
    /// </summary>
    public void SpawnEnemy()
    {
        if (player == null) return;

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
            Debug.LogWarning("EnemyManager: Spawned object does not have an Enemy component!");
        }
    }

    /// <summary>
    /// Escolhe qual prefab spawnar baseado em kills.
    /// </summary>
    GameObject ChooseEnemyPrefab()
    {
        if (killCount < 20)
        {
            return enemyPrefabs[0];
        }
        else if (killCount < 30)
        {
            return enemyPrefabs[UnityEngine.Random.Range(0, 2)];
        }

        else
        {
            return enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        }
    }

    /// <summary>
    /// Conta um kill e pode acelerar dificuldade.
    /// </summary>
    public void RegisterKill()
    {
        killCount++;
        Debug.Log("EnemyManager: Total Kills = " + killCount);

        // Atualiza HUD
        if (killUI != null)
        {
            killUI.UpdateKillCount(killCount);
        }
        
        // Acelera spawn a cada 20 kills
        if (killCount % 25 == 0 && spawnInterval > 0.5f)
        {
            spawnInterval -= 0.2f;
            Debug.Log("Spawn interval acelerated: " + spawnInterval);
        }
    }
}