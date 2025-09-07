using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform playerModel;

    Animator animator;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = playerModel.GetComponent<Animator>();
        spriteRenderer = playerModel.GetComponent<SpriteRenderer>();
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
        
        // Test death with K
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }

        // Test revive with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            Revive();
        }
    }

    public void Die()
    {
        animator.SetBool("IsDead", true); // switch to Dead state
    }

    public void Revive()
    {
        animator.SetBool("IsDead", false); // return to Idle
    }
}
