using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 velocity = new();
    private float speed = 0;

    public void Initialize(Vector3 aVelocity, float aSpeed)
    {
        Destroy(gameObject, 5.0f);          // destrói projétil depois de 5s
        transform.up = aVelocity;           // orienta o sprite na direção
        velocity = aVelocity * aSpeed;      // define a velocidade
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // checa se acertou um inimigo
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);            // causa dano no inimigo
            Destroy(gameObject);             // destrói o projétil
        }
    }
}