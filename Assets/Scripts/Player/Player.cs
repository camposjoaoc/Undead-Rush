using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerModel;

    [SerializeField] private Transform HandRight;
    [SerializeField] private Transform HandLeft;
    [SerializeField] private GameObject startingWeaponPrefab;

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float maxHealth = 5.0f;
    private float currentHealth;

    private Weapon currentWeapon;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = playerModel.GetComponent<Animator>();
        spriteRenderer = playerModel.GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // instancia a arma inicial na mão direita
        GameObject weaponInstance = Instantiate(startingWeaponPrefab, HandRight);
        currentWeapon = weaponInstance.GetComponent<Weapon>();
        currentWeapon.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        // movimento com WASD
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(x, y, 0f);
        transform.position += movement.normalized * (moveSpeed * Time.deltaTime);

        // animação de idle/run
        float speed = movement.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        // posição do mouse em coordenadas de mundo
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // flip do sprite baseado na posição do mouse (não no movimento)
        bool lookingLeft = mousePos.x < transform.position.x;
        spriteRenderer.flipX = lookingLeft;

        if (currentWeapon != null)
        {
            if (lookingLeft)
            {
                // Player olhando pra esquerda → arma na mão esquerda
                currentWeapon.transform.SetParent(HandLeft, false);
                currentWeapon.transform.localPosition = Vector3.zero;

                // espelha a arma
                Vector3 scale = currentWeapon.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                currentWeapon.transform.localScale = scale;
            }
            else
            {
                // Player olhando pra direita → arma na mão direita
                currentWeapon.transform.SetParent(HandRight, false);
                currentWeapon.transform.localPosition = Vector3.zero;

                // arma normal
                Vector3 scale = currentWeapon.transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                currentWeapon.transform.localScale = scale;
            }
        }

        // tiro
        if (Input.GetMouseButtonDown(0) && currentWeapon != null)
        {
            currentWeapon.Shoot(mousePos);
        }

        // debug vida
        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.R)) Revive();
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
        Destroy(gameObject);
        animator.SetBool("isDead", true);
        Debug.Log("Player morreu!");
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        animator.SetBool("isDead", false);
        Debug.Log("Player reviveu!");
    } 
}
