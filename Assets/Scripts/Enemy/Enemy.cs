using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;         // Ref to the player
    [SerializeField] private Transform graphics;       // Sprite/animator for the enemy
    [SerializeField] private float speed = 3f;         // Movement speed
    [SerializeField] private float stopRadius = 0.1f;  // Distance to stop from the player

    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 3;        // Vida máxima
    private int currentHealth;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = graphics.GetComponent<Animator>();
        spriteRenderer = graphics.GetComponent<SpriteRenderer>();
        currentHealth = maxHealth; // começa com vida cheia

        // Procura o player se não foi atribuído
        if (player == null)
        {
            Player playerRef = GameObject.FindAnyObjectByType<Player>();
            if (playerRef != null)
            {
                player = playerRef.transform;
                Debug.Log("Enemy found player: " + player.name);
            }
            else
            {
                Debug.LogWarning("No Player found in scene!");
            }
        }
    }

    void Update()
    {
        if (player == null) return;
        if (animator != null && animator.GetBool("isDead")) return;

        // Move towards the player
        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > stopRadius)
        {
            Vector3 direction = toPlayer.normalized;
            transform.position += direction * (speed * Time.deltaTime);

            // flip no sprite
            if (Mathf.Abs(direction.x) > 0.01f)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"Enemy took {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // toca animação de "hit" se existir
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    public void Die()
    {
        if (animator != null)
        {
            animator.SetBool("isDead", true);
            Destroy(gameObject, 1f); // espera animação de morte
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
