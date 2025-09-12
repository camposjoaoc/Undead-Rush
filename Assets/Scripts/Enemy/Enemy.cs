using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")] [SerializeField] private float moveSpeed = 3f; // Velocidade de movimento do inimigo
    [SerializeField] private float maxHealth = 3f; // Vida máxima do inimigo

    private float currentHealth; // Vida atual (decresce ao tomar dano)
    private Transform playerTarget; // Referência ao Player (passada pelo Manager)
    private Animator animator; // Controla animações (Idle, Run, Hit, Dead)
    private SpriteRenderer spriteRenderer; // Responsável por flipar o sprite esquerda/direita

    private float attackCooldown = 1f; // Tempo entre ataques
    private float lastAttackTime = 0f; // Tempo do último ataque

    /// <summary>
    /// Inicializa o inimigo quando for spawnado pelo EnemyManager.
    /// </summary>
    public void Initialize(Transform target)
    {
        playerTarget = target;
        currentHealth = maxHealth;

        // Busca os componentes gráficos no filho
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Atualiza o movimento do inimigo (chamado pelo Manager a cada frame).
    /// </summary>
    public void UpdateEnemy()
    {
        if (!IsAlive() || playerTarget == null)
            return;

        // Calcula direção até o player
        Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
        transform.position += toPlayer * moveSpeed * Time.deltaTime;

        // Animação Run/Idle + flip horizontal
        if (Mathf.Abs(toPlayer.x) > 0.01f)
        {
            spriteRenderer.flipX = toPlayer.x < 0;
        }
    }

    /// <summary>
    /// Aplica dano no inimigo e verifica se morreu.
    /// </summary>
    public void TakeDamage(float someDamage)
    {
        if (!IsAlive()) return;

        currentHealth -= someDamage;

        if (currentHealth <= 0)
        {
            // Troca para animação de morte
            animator?.SetBool("isDead", true);
            Destroy(gameObject, 1.1f);
        }
        else
        {
            // Animação de impacto quando leva dano
            animator?.SetTrigger("Hit");
        }
    }

    /// <summary>
    /// Retorna true se o inimigo ainda está vivo.
    /// </summary>
    public bool IsAlive() => currentHealth > 0;

    /// <summary>
    /// Detecta colisão com o Player para causar dano.
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsAlive()) return;

        Player player = collision.GetComponent<Player>();
        if (player != null && Time.time > lastAttackTime + attackCooldown)
        {
            player.TakeDamage(1); // Aplica dano no player
            lastAttackTime = Time.time;
        }
    }
}