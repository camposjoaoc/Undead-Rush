using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public int damage = 1;
    private Vector3 velocity = new();
    private float speed = 0;

    public void Initialize(Vector3 aVelocity, float aSpeed)
    {
        Destroy(gameObject, 5.0f); // destrói projétil depois de 5s
        transform.up = aVelocity; // orienta o sprite na direção
        speed = aSpeed; // define a velocidade
        velocity = aVelocity.normalized; // direção normalizada
    }

    private void Update()
    {
        // aplica movimento usando a velocidade armazenada
        transform.position += velocity * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // checa se acertou um inimigo
        Enemy enemy = other.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // causa dano no inimigo
            Destroy(gameObject); // destrói o projétil
        }
    }
}