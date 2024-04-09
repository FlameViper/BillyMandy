using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int baseHealth = 100; // Renamed to baseHealth for clarity
    public int currentHealth;
    public int damageToPlayer = 10;
    public float stopDistance = 1f;
    public float attackSpeed = 1f; // Attack speed in seconds

    public GameObject coinPrefab;

    private Transform player;
    private Coroutine attackRoutine = null;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = baseHealth; // Ensure current health is set to base at start
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
            attackRoutine = StartCoroutine(DealDamageRepeatedly(collision));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    IEnumerator DealDamageRepeatedly(Collider2D playerCollider)
    {
        while (true)
        {
            playerCollider.GetComponent<Player>().TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
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
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Add this method to set the enemy's health based on the current level
    public void SetHealth(int level)
    {
        baseHealth = 100 + (level - 1) * 20; // Increase base health by 20 for each level beyond the first
        currentHealth = baseHealth; // Reset current health to the new base health
    }
}
