using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int baseHealth = 30; // Renamed to baseHealth for clarity
    public int currentHealth;
    public int damageToPlayer = 10;
    public float stopDistance = 1f;
    public float attackSpeed = 1f; // Attack speed in seconds

    public GameObject coinPrefab;

    private Transform player;
    private Coroutine attackRoutine = null;
    public bool isFrozen = false;
    



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = baseHealth; // Ensure current health is set to base at start
    }

    void Update()
    {
        if (isFrozen) return; // Prevent moving when frozen

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    public void Freeze(bool freezeStatus)
    {
        isFrozen = freezeStatus;

        // Access the SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Change color to blue when frozen, revert to white when not frozen
            spriteRenderer.color = freezeStatus ? Color.blue : Color.white;
        }

        // If freezing, start the unfreeze coroutine
        if (freezeStatus)
        {
            StartCoroutine(UnfreezeAfterDuration(SupportProjectile.freezeDuration));
        }
    }

    IEnumerator UnfreezeAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Freeze(false); // Unfreeze the enemy after the specified duration
    }





    IEnumerator DealDamageRepeatedly(Collider2D playerCollider)
    {
        while (!isFrozen)
        {
            playerCollider.GetComponent<Player>().TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
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
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddScore(100); // Assuming each enemy kill gives you 100 points base
            
        }
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }



    // Add this method to set the enemy's health based on the current level
    public void SetHealth(int level)
    {
        int healthIncrease = 20; // Default for Easy

        switch (GameSettings.Instance.currentDifficulty)
        {
            case GameSettings.Difficulty.Medium:
                healthIncrease = 30;
                break;
            case GameSettings.Difficulty.Hard:
                healthIncrease = 50;
                break;
        }

        baseHealth = 30 + (level - 1) * healthIncrease;
        currentHealth = baseHealth;
    }

}
