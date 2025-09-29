using Mono.Cecil;
using UnityEngine;

public class TridentWeapon : MonoBehaviour
{
    [SerializeField] private GameObject tridentProjectilePrefab; // Prefab do projétil
    [SerializeField] private float cooldown = 10f; // Tempo de recarga entre os disparos 10s
    [SerializeField] private float projectileSpeed = 8f; // Velocidade do projétil

    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Fire();
            timer = cooldown;
        }
    }

    private void Fire()
    {
        Player player = GetComponentInParent<Player>();
        if (player == null) return;
        
        // Determina a direção do disparo com base na orientação do jogador
        bool facingLeft = player.GetComponentInChildren<SpriteRenderer>().flipX;
        Vector3 fireDirection = facingLeft ? Vector3.left : Vector3.right;
        
        // Instancia o projétil na posição do jogador
        GameObject ptridentObj = Instantiate(tridentProjectilePrefab, player.transform.position, Quaternion.identity);
        
        // Inicializa o projétil com a direção e velocidade
        Projectile tridentProjectile = ptridentObj.GetComponent<Projectile>();
        if (tridentProjectile != null)
        {
            tridentProjectile.Initialize(fireDirection, projectileSpeed);
        }
        else
        {
            Debug.LogWarning("Projectile component missing on trident prefab!");
            Destroy(ptridentObj);
        }
    }
}