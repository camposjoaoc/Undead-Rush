using System;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    [SerializeField] private float orbitRadius = 1.5f; // Raio da órbita
    [SerializeField] private float orbitSpeed = 90f; // Graus por segundo
    [SerializeField] private int damage = 5;

    private Transform player;
    private float angle;

    public void Initialize(Transform playerTarget, float startAngle)
    {
        player = playerTarget;
        angle = startAngle* Mathf.Deg2Rad;
    }

    public void Update()
    {
        if (GamesManager.Instance.CurrentState != GamesManager.GameState.Playing)
            return;
        
        if (player == null) return;
        
        // Avança o ângulo baseado na velocidade (graus convertidos para radianos)
        angle += (orbitSpeed * Mathf.Deg2Rad) * Time.deltaTime;

        // Nova posição no círculo
        Vector3 orbitPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbitRadius;
        transform.position = player.position + orbitPos;

        // Direção tangente ao círculo
        Vector3 tangent = new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0);
        transform.right = -tangent; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.isDead)
        {
            enemy.TakeDamage(damage);
        }
    }
}