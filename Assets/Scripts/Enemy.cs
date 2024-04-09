using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHealth = 100;
    public int currentHealth;
    public int damageToPlayer = 10;
    public float stopDistance = 1f;
    public float attackSpeed = 1f; // Attack speed in seconds

    public GameObject coinPrefab; // Add this line to reference the coin prefab

    private Transform player;
    private Coroutine attackRoutine = null; // To keep track of the attack coroutine

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && attackRoutine == null)
        {
            // Start dealing damage repeatedly when the enemy enters the player's hitbox
            attackRoutine = StartCoroutine(DealDamageRepeatedly(collision));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && attackRoutine != null)
        {
            // Stop dealing damage when the enemy exits the player's hitbox
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    IEnumerator DealDamageRepeatedly(Collider2D playerCollider)
    {
        while (true)
        {
            playerCollider.GetComponent<Player>().TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed); // Wait for attackSpeed seconds before dealing damage again
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
        // Instantiate the coin prefab at the enemy's position before destroying the enemy
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
