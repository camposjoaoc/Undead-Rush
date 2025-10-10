using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public int damage = 1;
    private Vector3 velocity = new();
    private float speed = 0;

    public void Initialize(Vector3 aVelocity, float aSpeed)
    {
        Destroy(gameObject, 5.0f); // destroy projectile after 5s
        transform.up = aVelocity; // sprite direction
        speed = aSpeed; // defines velocity
        velocity = aVelocity.normalized; // normalized direction
    }

    private void Update()
    {
        //apply movement using stored velocity
        transform.position += velocity * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if an enemy was hit
        Enemy enemy = other.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // enemy takes damage
            Destroy(gameObject); // destroy projectile
        }
    }
}