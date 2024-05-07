using System.Collections;
using UnityEngine;


public class EnemyShooter : Enemy
{
    public GameObject projectilePrefab;
    public float attackRange = 10f; // Range within which the enemy can shoot
    private float lastAttackTime = 0f;


    protected override void Start() {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (isFrozen || player == null) return; // Prevent shooting when frozen or if player is null

        // Shooting logic
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (Time.time - lastAttackTime >= 1f / attackSpeed)
            {
                ShootProjectile();
                lastAttackTime = Time.time;
            }
        }
    }

    void ShootProjectile()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null)
        {
            enemyProjectile.SetDirection(direction);
        }
    }

  


}
