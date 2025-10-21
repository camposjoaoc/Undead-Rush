using UnityEngine;

public class TridentWeapon : MonoBehaviour
{
    [SerializeField] private GameObject tridentProjectilePrefab; // Projectile prefab
    [SerializeField] private float cooldown = 5f; // Cooldown between shoots (5s)
    [SerializeField] private float projectileSpeed = 8f; // Projectile velocity

    private float timer;

    private void Update()
    {
        if (GamesManager.Instance.CurrentState != GamesManager.GameState.Playing)
            return;

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
        
        // Determine shooting direction based on players orientation
        bool facingLeft = player.GetComponentInChildren<SpriteRenderer>().flipX;
        Vector3 fireDirection = facingLeft ? Vector3.left : Vector3.right;
        
        // Instantiate projectile at players position
        GameObject ptridentObj = Instantiate(tridentProjectilePrefab, player.transform.position, Quaternion.identity);
        
        // Initialize projectile with direction and velocity
        Projectile tridentProjectile = ptridentObj.GetComponent<Projectile>();
        if (tridentProjectile != null)
        {
            tridentProjectile.Initialize(fireDirection, projectileSpeed);
        }
        else
        {
            Debug.LogWarning("[TridentWeapon] Projectile component missing on trident prefab!");
            Destroy(ptridentObj);
        }
    }
}