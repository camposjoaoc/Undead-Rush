using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Damageable
{
    [Header("References")] [SerializeField]
    Transform playerModel;

    [SerializeField] private Transform HandRight;
    [SerializeField] private Transform HandLeft;
    [SerializeField] private GameObject startingWeaponPrefab;

    [Header("Secondary Weapons")] [SerializeField]
    private GameObject shovelPrefab;

    private List<Shovel> activeShovels = new List<Shovel>();
    private int maxShovels = 8;

    [SerializeField] private GameObject tridentWeaponPrefab;
    private TridentWeapon tridentWeapon;
    public bool HasTrident => tridentWeapon != null;

    [Header("PlayerUI")] 
    [SerializeField] Image HealthBar;
    [SerializeField] TextMeshProUGUI HealthText;

    [Header("Movimento")] [SerializeField] float moveSpeed = 3f;

    private Weapon currentWeapon;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake(); // Initialize currentHealth = maxHealth (Damageable)
        animator = playerModel.GetComponent<Animator>();
        spriteRenderer = playerModel.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Instantiate initial weapon at right hand
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
        // Update HealthBar
        HealthText.text = currentHealth.ToString("F0") + " / " + maxHealth.ToString();
        HealthBar.fillAmount = currentHealth / maxHealth;
    }

    private void HandleMovementAndAnimation()
    {
        // Movement with WASD
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(x, y, 0f);
        transform.position += movement.normalized * (moveSpeed * Time.deltaTime);

        // idle/run animation
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
        // Mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Direction between player and mouse
        Vector3 aimDir = (mousePos - transform.position).normalized;
        bool lookingLeft = (mousePos.x < transform.position.x);
        spriteRenderer.flipX = lookingLeft;

        if (currentWeapon == null) return;

        // Switch hand
        currentWeapon.transform.SetParent(lookingLeft ? HandLeft : HandRight, false);
        currentWeapon.transform.localPosition = Vector3.zero;

        // Weapon rotation
        currentWeapon.transform.right = aimDir;
        
        // Fix weapon vertical flip when player look left
        Vector3 localScale = currentWeapon.transform.localScale;
        localScale.y = lookingLeft ? -1 : 1;
        currentWeapon.transform.localScale = localScale;
    }

    private void HandleWeapon()
    {
        if (currentWeapon == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Shoot
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
        Debug.Log("[Player] Player died!");

        SoundManager.Instance.PlaySoundEffect(SoundEffects.PlayerDeath);

        UpdateHealthUI();
        GamesManager.Instance.SwitchState(GamesManager.GameState.GameOver);
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        isDead = false;
        animator.SetBool("isDead", false);
        Debug.Log("[Player] Player undead!");
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
    
    public int CurrentShovelCount => activeShovels.Count;
    public int MaxShovels => maxShovels;
    
}