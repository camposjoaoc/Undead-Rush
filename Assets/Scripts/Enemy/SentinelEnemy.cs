using UnityEngine;

public class SentinelEnemy : Enemy
{
    [SerializeField] private float detectionRange = 3f; // Player detection range
    [SerializeField] private float chargeSpeed = 2.5f; // Speed while charging

    private bool isCharging = false;

    public override void UpdateEnemy()
    {
        {
            if (isDead || playerTarget == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (!isCharging && distanceToPlayer <= detectionRange)
            {
                // Start charging
                isCharging = true;
            }

            if (isCharging)
            {
                // Run towards the player
                Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
                transform.position += toPlayer * chargeSpeed * Time.deltaTime;
                
                spriteRenderer.flipX = toPlayer.x < 0;

                //Animation "Run"
                animator?.SetFloat("Speed", 1f);
            }
            else
            {
                // Animation "Idle"
                animator?.SetFloat("Speed", 0f);
            }
        }
    }
}