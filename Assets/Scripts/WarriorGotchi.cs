using System.Collections.Generic;
using UnityEngine;
using System;

public class WarriorGotchi : MonoBehaviour
{
    public float detectionRadius = 10f;
    public float moveSpeed = 3f;
    public float attackInterval = 1f;

    public static int baseHealth = 50; // Default health
    public static int baseDamage = 5;  // Default damage

    private int currentHealth;
    private int attackDamage;

    private static List<Transform> currentTargets = new List<Transform>(); // Static list shared among all instances
    private Transform player;
    private Transform targetEnemy = null;
    private float lastAttackTime = 0;
    private BoxCollider2D myCollider;

    public Action OnDestroyAction;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        myCollider = GetComponent<BoxCollider2D>();  // Assuming a BoxCollider2D is attached to WarriorGotchi
        currentHealth = baseHealth;
        attackDamage = baseDamage;
    }

    void Update()
    {
        UpdateTargetEnemy();

        if (targetEnemy != null)
        {
            MoveTowardsTarget();
            if (Vector2.Distance(transform.position, targetEnemy.position) <= GetStoppingDistance())
            {
                Attack();
            }
        }
    }

    void UpdateTargetEnemy()
    {
        if (targetEnemy == null || !currentTargets.Contains(targetEnemy))
        {
            float closestDistance = detectionRadius;
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                float distance = Vector2.Distance(player.position, enemy.transform.position);
                if (distance <= detectionRadius && distance < closestDistance && !currentTargets.Contains(enemy.transform))
                {
                    if (targetEnemy != null)
                    {
                        currentTargets.Remove(targetEnemy);
                    }
                    targetEnemy = enemy.transform;
                    closestDistance = distance;
                    if (!currentTargets.Contains(targetEnemy))
                    {
                        currentTargets.Add(targetEnemy);
                    }
                }
            }
        }
    }

    private void MoveTowardsTarget()
    {
        float stoppingDistance = GetStoppingDistance();
        if (Vector2.Distance(transform.position, targetEnemy.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
        }
    }

    private float GetStoppingDistance()
    {
        if (targetEnemy != null)
        {
            BoxCollider2D targetCollider = targetEnemy.GetComponent<BoxCollider2D>(); // Get the enemy's collider
            if (targetCollider != null)
            {
                // Subtract a small buffer from the sum of half the widths to allow closer approach
                float buffer = 0.6f; // Adjust this value to decrease/increase stopping distance
                return Mathf.Max(0, (myCollider.size.x / 2 + targetCollider.size.x / 2) - buffer);
            }
        }
        return 0f;
    }


    private void Attack()
    {
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;
            targetEnemy.GetComponent<Enemy>().TakeDamage(attackDamage,false);
        }
    }

    // Method to increase health
    public static void IncreaseHealth(int amount)
    {
        baseHealth += amount;
    }

    // Method to increase damage
    public static void IncreaseDamage(int amount)
    {
        baseDamage += amount;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Use currentHealth instead of health
        if (currentHealth <= 0)
        {
            if (targetEnemy != null)
            {
                currentTargets.Remove(targetEnemy);
            }
            Destroy(gameObject);  // Properly destroy the WarriorGotchi instance
        }
    }


    void OnDestroy()
    {
        OnDestroyAction?.Invoke();
        if (targetEnemy != null)
        {
            currentTargets.Remove(targetEnemy);
        }
    }
}
