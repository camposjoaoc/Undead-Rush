using System;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    [SerializeField] private float orbitRadius = 1.5f; // Orbit radius
    [SerializeField] private float orbitSpeed = 90f; // Grades per second
    [SerializeField] private int damage = 3;

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
        
        // Increase angle based on velocity 
        angle += (orbitSpeed * Mathf.Deg2Rad) * Time.deltaTime;

        // New circle position
        Vector3 orbitPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbitRadius;
        transform.position = player.position + orbitPos;
        
        // Tangent direction to circle
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