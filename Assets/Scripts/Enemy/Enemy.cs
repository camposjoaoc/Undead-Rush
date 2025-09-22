using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Damageable
{
    [Header("Stats")] 
    [SerializeField] private float moveSpeed = 3f; // Velocidade de movimento do inimigo
    protected Transform playerTarget; // Referência ao Player (passada pelo Manager)
    protected Animator animator; // Controla animações (Idle, Run, Hit, Dead)
    protected SpriteRenderer spriteRenderer; // Responsável por flipar o sprite esquerda/direita

    private float attackCooldown = 1f; // Tempo entre ataques
    private float lastAttackTime = 0f; // Tempo do último ataque
    
    /// Inicializa o inimigo quando for spawnado pelo EnemyManager.
    public void Initialize(Transform target)
    {
        playerTarget = target;
        currentHealth = maxHealth;
        isDead = false;

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        
        animator?.SetBool("isDead", true);
        Destroy(gameObject, 1.1f);
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
        }
    }
}