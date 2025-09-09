using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform playerModel;
    [SerializeField] int maxHealth = 5;

    private int currentHealth;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = playerModel.GetComponent<Animator>();
        spriteRenderer = playerModel.GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // old Input system (WASD / arrow keys)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // movement vector
        Vector3 movement = new Vector3(x, y, 0f);
        transform.position += movement.normalized * (moveSpeed * Time.deltaTime);

        // update Idle/Run animations
        float speed = movement.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        // flip sprite left/right
        if (Mathf.Abs(x) > 0.01f)
        {
            spriteRenderer.flipX = x < 0;
        }
        
        // Debug test
        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(1); // perde 1 de vida
        if (Input.GetKeyDown(KeyCode.R)) Revive();      // revive
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; // já morto

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetBool("IsDead", true); // switch to Dead state
        Debug.Log("Player morreu!");
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        animator.SetBool("IsDead", false); // return to Idle
        Debug.Log("Player reviveu!");
    }
}