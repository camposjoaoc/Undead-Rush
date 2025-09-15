using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerModel;

    [SerializeField] private Transform HandRight;
    [SerializeField] private Transform HandLeft;
    [SerializeField] private GameObject startingWeaponPrefab;

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float maxHealth = 5f;

    [SerializeField] Image HealthBar;
    [SerializeField] TextMeshProUGUI HealthText;
    [SerializeField] float currentHealth;

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

    public void TakeDamage(int amount)
    {
        // não aplica dano se já estiver morto
        if (currentHealth <= 0) return;
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        animator.SetBool("IsDead", true);
        Debug.Log("Player morreu!");
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        animator.SetBool("IsDead", false);
        Debug.Log("Player reviveu!");
    }
}