using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : Damageable
{
    [Header("Referências")]
    [SerializeField] Transform playerModel;
    [SerializeField] private Transform HandRight;
    [SerializeField] private Transform HandLeft;
    [SerializeField] private GameObject startingWeaponPrefab;

    [Header("PlayerUI")]
    [SerializeField] Image HealthBar;
    [SerializeField] TextMeshProUGUI HealthText;
    
    [Header("Movimento")]
    [SerializeField] float moveSpeed = 3f;

    private Weapon currentWeapon;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake(); // inicializa currentHealth = maxHealth (Damageable)
        animator = playerModel.GetComponent<Animator>();
        spriteRenderer = playerModel.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // instancia a arma inicial na mão direita
        GameObject weaponInstance = Instantiate(startingWeaponPrefab, HandRight);
        currentWeapon = weaponInstance.GetComponent<Weapon>();
        currentWeapon.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        // atualiza barra de vida
        HealthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString();
        HealthBar.fillAmount = currentHealth / maxHealth;

        // movimento com WASD
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(x, y, 0f);
        transform.position += movement.normalized * (moveSpeed * Time.deltaTime);

        // animação de idle/run
        float speed = movement.sqrMagnitude;
        animator.SetFloat("Speed", speed);
        
        // posição do mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // direção entre player e mouse
        Vector3 aimDir = (mousePos - transform.position).normalized;

        // flip do player baseado no mouse
        bool lookingLeft = (mousePos.x < transform.position.x);
        spriteRenderer.flipX = lookingLeft;

        // troca de mão da arma
        if (currentWeapon != null)
        {
            if (lookingLeft)
            {
                currentWeapon.transform.SetParent(HandLeft, false);
                currentWeapon.transform.localPosition = Vector3.zero;
            }
            else
            {
                currentWeapon.transform.SetParent(HandRight, false);
                currentWeapon.transform.localPosition = Vector3.zero;
            }

            // rotação da arma
            currentWeapon.transform.right = aimDir;

            // corrige flip vertical da arma quando player olha pra esquerda
            Vector3 localScale = currentWeapon.transform.localScale;
            localScale.y = lookingLeft ? -1 : 1;
            currentWeapon.transform.localScale = localScale;
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

    protected override void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        Debug.Log("Player morreu!");
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        isDead = false;
        animator.SetBool("isDead", false);
        Debug.Log("Player reviveu!");
    }
}