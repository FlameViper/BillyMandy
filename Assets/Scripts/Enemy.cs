using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int baseHealth = 30;
    public int currentHealth;
    public int damageToPlayer = 10;
    public float stopDistance = 1f;
    public float attackSpeed = 1f;
    public GameObject coinPrefab;

    public AudioSource damageSound; // AudioSource for playing damage sound effect
    public AudioSource deathSound;

    private Transform player;
    private Coroutine attackRoutine = null;
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer

    private bool isDead = false; // Flag to check if the enemy is dead
    private bool isFrozen = false; // Track freeze state
    private Coroutine freezeCoroutine; // To manage ongoing freeze coroutines


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = baseHealth;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Update()
    {
        if (isDead || isFrozen) return; // Stop all movement if enemy is dead or frozen

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
        if (isFrozen == freezeStatus) return;  // No change in status

        isFrozen = freezeStatus;
        spriteRenderer.color = freezeStatus ? Color.blue : Color.white;

        if (freezeStatus)
        {
            if (freezeCoroutine != null)
            {
                StopCoroutine(freezeCoroutine);  // Ensure no overlapping coroutines
            }
            // Use the static freezeDuration from SupportProjectile
            freezeCoroutine = StartCoroutine(UnfreezeAfterDuration(SupportProjectile.freezeDuration));
        }
        else if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }
    }

    private IEnumerator UnfreezeAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Freeze(false);  // Automatically unfreeze after duration
    }



    IEnumerator DealDamageRepeatedly(Collider2D playerCollider)
    {
        while (!isFrozen && !isDead)
        {
            playerCollider.GetComponent<Player>().TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && attackRoutine == null && !isDead)
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
        Debug.Log("TakeDamage called. Current health: " + currentHealth + ", Damage: " + damage);

        if (isDead || isFrozen) return; // Ignore damage if already dead or frozen

        currentHealth -= damage;

        // Play damage sound effect
        if (damageSound != null)
        {
            damageSound.Play();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Prevent multiple deaths
        isDead = true; // Mark as dead

        // Disable all colliders on the enemy
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        // Play death sound effect
        if (deathSound != null)
        {
            deathSound.Play();
        }

        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddScore(100);
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);

        StartCoroutine(FadeOut(1f)); // Fade out over 1 second
    }


    IEnumerator FadeOut(float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }
        Destroy(gameObject); // Destroy the object after fading out
    }

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