using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Damageable
{
    [Header("Stats")] [SerializeField] private float moveSpeed = 3f; // Velocidade de movimento do inimigo
    protected Transform playerTarget; // Referência ao Player (passada pelo Manager)
    protected Animator animator; // Controla animações (Idle, Run, Hit, Dead)
    protected SpriteRenderer spriteRenderer; // Responsável por flipar o sprite esquerda/direita

    private static readonly int IsDeadHash = Animator.StringToHash("isDead");
    
    private float attackCooldown = 1f; 
    private float lastAttackTime = 0f; 

    private Coroutine deathRoutine;
    private bool _pendingAnimatorReset; 
    protected new void Awake()
    {
        base.Awake(); 
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    /// Inicializa o inimigo quando for spawnado pelo EnemyManager.
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

            // se já estiver ativo, podemos atualizar agora; caso contrário, adia
            if (gameObject.activeInHierarchy && animator.isActiveAndEnabled)
            {
                animator.Update(0f);
                animator.speed = 1f;
                _pendingAnimatorReset = false;
            }
            else
            {
                _pendingAnimatorReset = true;
            }
        }
    }
    private void OnEnable()
    {
        // completa o reset se Initialize foi chamado com GO inativo
        if (_pendingAnimatorReset && animator != null && animator.isActiveAndEnabled)
        {
            animator.Update(0f);
            animator.speed = 1f;
            _pendingAnimatorReset = false;
        }
    }
    
    /// Atualiza o movimento do inimigo (chamado pelo Manager a cada frame).
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

    /// Comportamento de morte do inimigo (sobrescreve Damageable).
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
        // ajuste para o comprimento real do seu clip "Enemy_Death"
        yield return new WaitForSeconds(1f);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        deathRoutine = null;
    }

    private void OnDisable()
    {
        // garante que nenhuma rotina de morte fique viva ao desativar (pool/scene change)
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

    /// Detecta colisão com o Player para causar dano.
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