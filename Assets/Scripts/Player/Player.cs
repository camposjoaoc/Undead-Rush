using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Damageable
{
    [Header("Referências")] 
    [SerializeField] Transform playerModel;

    [SerializeField] private Transform HandRight;
    [SerializeField] private Transform HandLeft;
    [SerializeField] private GameObject startingWeaponPrefab;
    
    [Header("Armas Secundárias")] 
    [SerializeField] private GameObject shovelPrefab; // Prefab da pá
    private List<Shovel> activeShovels = new List<Shovel>();
    private int maxShovels = 4;
    
    [SerializeField] private GameObject tridentWeaponPrefab; // Prefab do tridente
    private TridentWeapon tridentWeapon;
    
    [Header("PlayerUI")] 
    [SerializeField] Image HealthBar;
    [SerializeField] TextMeshProUGUI HealthText;

    [Header("Movimento")] [SerializeField] float moveSpeed = 3f;

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


    public void UpdatePlayer()
    {
        UpdateHealthUI();
        HandleMovementAndAnimation();
        HandleAimingAndWeapon();
        HandleDebugInputs();
    }

    private void UpdateHealthUI()
    {
        // atualiza barra de vida
        HealthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString();
        HealthBar.fillAmount = currentHealth / maxHealth;
    }

    private void HandleMovementAndAnimation()
    {
        // movimento com WASD
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(x, y, 0f);
        transform.position += movement.normalized * (moveSpeed * Time.deltaTime);

        // animação de idle/run
        float speed = movement.sqrMagnitude;
        animator.SetFloat("Speed", speed);
    }

    private void HandleAimingAndWeapon()
    {
        HandleAiming();
        HandleWeapon();
    }

    private void HandleAiming()
    {
        // posição do mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // direção entre player e mouse
        Vector3 aimDir = (mousePos - transform.position).normalized;
        bool lookingLeft = (mousePos.x < transform.position.x);
        spriteRenderer.flipX = lookingLeft;

        if (currentWeapon == null) return;

        // troca de mão
        currentWeapon.transform.SetParent(lookingLeft ? HandLeft : HandRight, false);
        currentWeapon.transform.localPosition = Vector3.zero;

        // rotação da arma
        currentWeapon.transform.right = aimDir;

        // corrige flip vertical da arma quando player olha pra esquerda
        Vector3 localScale = currentWeapon.transform.localScale;
        localScale.y = lookingLeft ? -1 : 1;
        currentWeapon.transform.localScale = localScale;
    }

    private void HandleWeapon()
    {
        if (currentWeapon == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // tiro
        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.Shoot(mousePos);
            SoundManager.Instance.PlaySoundEffect(SoundEffects.PlayerShoot);
        }
    }

    private void HandleDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.R)) Revive();
    }

    protected override void Die()
    {
        currentHealth = 0;
        isDead = true;
        animator.SetBool("isDead", true);
        Debug.Log("[Player] Player morreu!");
        
        SoundManager.Instance.PlaySoundEffect(SoundEffects.PlayerDeath);
        
        UpdateHealthUI();
        GamesManager.Instance.SwitchState(GamesManager.GameState.GameOver);
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        isDead = false;
        animator.SetBool("isDead", false);
        Debug.Log("[Player] Player reviveu!");
    }

    // Upgrades
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void UnlockSecondaryWeapon()
    {
        if (activeShovels.Count >= maxShovels) return;
        GameObject shovelWeapon = Instantiate(shovelPrefab, transform.position, Quaternion.identity);
        Shovel shovel = shovelWeapon.GetComponent<Shovel>();
        activeShovels.Add(shovel);
        
        float angleStep = 360f / activeShovels.Count;
        for (int i = 0; i < activeShovels.Count; i++)
        {
            activeShovels[i].Initialize(transform, i * angleStep);
        }
    }
    
    public void UnlockTertiaryWeapon()
    {
        if (tridentWeapon == null)
        {
            GameObject tridentInstance = Instantiate(tridentWeaponPrefab, transform);
            tridentWeapon = tridentInstance.GetComponent<TridentWeapon>();
        }
    }
}