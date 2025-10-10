using UnityEngine;

public class Weapon : MonoBehaviour
{
   [SerializeField] private GameObject projectilePrefab;
   [SerializeField] private Transform firePoint; // Spot where projectile is shot
   [SerializeField] private float projectileSpeed = 8f; // Projectile velocity

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
         Debug.LogWarning("[Weapon] Projectile component missing on prefab!");
         Destroy(bullet);
      }
   }
}
