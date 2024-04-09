using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackSpeed = 1f; // Attacks per second
    public float attackRange = 10f; // Range within which the enemy can shoot

    public int baseHealth = 100; // Base health of the shooter enemy
    public int currentHealth;

    public GameObject coinPrefab; // Add this to reference the coin prefab

    private Transform player;
    private float lastAttackTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = baseHealth; // Initialize current health with base health
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    // Method to adjust health based on the game level
    public void SetHealth(int level)
    {
        baseHealth = 100 + (level - 1) * 20; // For example, increase base health by 20 for each level
        currentHealth = baseHealth;
    }
}
