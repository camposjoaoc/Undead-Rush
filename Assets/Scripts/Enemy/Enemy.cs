using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy that follows the player
    [SerializeField] Transform player;
    [SerializeField] private Transform graphics;
    
    [SerializeField] float speed = 3f; // Enemy movement speed (less than player speed)
    [SerializeField] float stopRadius = 0.1f; // Distance to stop from player (avoid overlap)
    
    Animator animator;
    SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        animator = graphics.GetComponent<Animator>();
        spriteRenderer = graphics.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        //If dead, do nothing
        if (animator != null && animator.GetBool("IsDead")) return; 
        
        // move towards player
        if (player == null) return;

        // direction vector from enemy to player
        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > stopRadius)
        {
            Vector3 direction = toPlayer.normalized;
            transform.position += direction * (speed * Time.deltaTime);
            
            // update Idle/Run animations
            if (Mathf.Abs(direction.x) > 0.01f)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        // update Run/Dead animations
        void Die()
        {
            if (animator != null)
            {
                animator.SetBool("isDead", true); // switch to Dead state
                Destroy(gameObject, 1.5f); // destroy enemy after delay
            }
        }
        
        // Call this method to simulate the enemy being hit
        void Hit()
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }
}
