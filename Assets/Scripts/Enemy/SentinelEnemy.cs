using UnityEngine;

public class SentinelEnemy : Enemy
{
    [SerializeField] private float detectionRange = 5f; // Raio de detecção do jogador
    [SerializeField] private float chargeSpeed = 3f; // Velocidade quando ataca

    private bool isCharging = false;

    public override void UpdateEnemy()
    {
        {
            if (isDead || playerTarget == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (!isCharging && distanceToPlayer <= detectionRange)
            {
                // Entrou no alcance, começa a perseguir
                isCharging = true;
            }

            if (isCharging)
            {
                // Corre em direção ao player
                Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
                transform.position += toPlayer * chargeSpeed * Time.deltaTime;

                // Flip do sprite
                spriteRenderer.flipX = toPlayer.x < 0;

                // Animação
                animator?.SetFloat("Speed", 1f);
            }
            else
            {
                // Idle
                animator?.SetFloat("Speed", 0f);
            }
        }
    }
}