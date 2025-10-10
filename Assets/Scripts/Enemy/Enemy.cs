using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Damageable
{
    [Header("Stats")] [SerializeField] private float moveSpeed = 3f; // Enemy movement speed
    protected Transform playerTarget; // Player reference
    protected Animator animator; // Animation controll (Idle, Run, Hit, Dead)
    protected SpriteRenderer spriteRenderer; // Flip sprite left/right

    private static readonly int IsDeadHash = Animator.StringToHash("isDead");
    
    private float attackCooldown = 1f; 
    private float lastAttackTime = 0f; 

    private Coroutine deathRoutine;
    private bool pendingAnimatorReset; 
    protected new void Awake()
    {
        base.Awake(); 
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    //Initialize enemy when spawned by EnemyManager 
    public void Initialize(Transform target)
    {
        playerTarget = target;
        currentHealth = maxHealth;
        isDead = false;

        StopDeathRoutineIfAny();

        if (animator != null)
        {
            animator.Rebind();
            animator.SetBool(IsDeadHash, false);
            
            if (gameObject.activeInHierarchy && animator.isActiveAndEnabled)
            {
                animator.Update(0f);
                animator.speed = 1f;
                pendingAnimatorReset = false;
            }
            else
            {
                pendingAnimatorReset = true;
            }
        }
    }
    private void OnEnable()
    {
        if (pendingAnimatorReset && animator != null && animator.isActiveAndEnabled)
        {
            animator.Update(0f);
            animator.speed = 1f;
            pendingAnimatorReset = false;
        }
    }
    
    public virtual void UpdateEnemy()
    {
        if (isDead || playerTarget == null) return;

        // Calcula direção até o player
        Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
        transform.position += toPlayer * moveSpeed * Time.deltaTime;

        // Animação Run/Idle + flip horizontal
        if (Mathf.Abs(toPlayer.x) > 0.01f)
        {
            spriteRenderer.flipX = toPlayer.x < 0;
        }
    }
    
    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetBool(IsDeadHash, true);

        SoundManager.Instance.PlaySoundEffect(SoundEffects.EnemyDeath);
        GamesManager.Instance.AddExperience(1);

        StopDeathRoutineIfAny();
        deathRoutine = StartCoroutine(DisableAfterAnimation());
    }

    private IEnumerator DisableAfterAnimation()
    {
        yield return new WaitForSeconds(1f);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        deathRoutine = null;
    }

    private void OnDisable()
    {
        StopDeathRoutineIfAny();
    }

    private void StopDeathRoutineIfAny()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }
    }
    
    // Detect collision with Player to cause damage
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;

        Player player = collision.GetComponent<Player>();
        if (player != null && Time.time > lastAttackTime + attackCooldown)
        {
            player.TakeDamage(1); // Aplica dano no player
            lastAttackTime = Time.time;
            SoundManager.Instance.PlaySoundEffect(SoundEffects.EnemyAttack);
        }
    }
}