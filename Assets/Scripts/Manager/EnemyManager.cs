using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Prefab do inimigo
    [SerializeField] private int maxEnemies = 300; // Quantos inimigos no total
    [SerializeField] private float spawnInterval = 2f; // Intervalo entre spawns
    [SerializeField] private Vector2 spawnRangeX = new(-10f, 10f); 
    [SerializeField] private Vector2 spawnRangeY = new(-5f, 5f); 

    private List<GameObject> enemies = new List<GameObject>();
    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        // Atualiza o timer
        timer -= Time.deltaTime;

        // Se já passou o tempo e ainda não chegou no limite
        if (timer <= 0 && enemies.Count < maxEnemies)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }

        // Remove referências de inimigos destruídos
        enemies.RemoveAll(enemy => enemy == null);
    }

    private void SpawnEnemy()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            Random.Range(spawnRangeY.x, spawnRangeY.y), // Usando Y (2D)
            0
        );

        GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        enemies.Add(newEnemy);
    }
}
