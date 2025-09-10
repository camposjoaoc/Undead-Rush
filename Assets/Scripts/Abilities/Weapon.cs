using UnityEngine;

public class Weapon : MonoBehaviour
{
   [SerializeField] private GameObject projectilePrefab; // Prefab of the projectile to shoot
   [SerializeField] private Transform firePoint; // Point from where the projectile is fired
   [SerializeField] private float projectileSpeed = 8f; // Speed of the projectile

   public void Shoot(Vector3 targetPosition)
   {
      if(projectilePrefab == null || firePoint == null)
      {
         Debug.LogWarning("Weapon missing prefab or firepoint!.");
         return;
      }
      
      Vector3 direction = (targetPosition - firePoint.position).normalized;
      
      GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
      
      Projectile proj = bullet.GetComponent<Projectile>();
      if (proj != null)
      {
         proj.Initialize(direction, projectileSpeed);
      }
      else
      {
         Debug.LogWarning("Projectile component missing on prefab!");
         Destroy(bullet);
      }
   }
}
