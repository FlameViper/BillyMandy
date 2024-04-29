using System.Collections;
using System.Collections.Generic;
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

    public AudioSource damageSound;
    public AudioSource deathSound;

    private Transform target;
    private Transform player;
    private Coroutine attackRoutine = null;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private bool isFrozen = false;
    private Coroutine freezeCoroutine;

    private List<Transform> potentialTargets = new List<Transform>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead || isFrozen) return;

        // Update target to the closest potential target
        UpdateTarget();

        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("WarriorGotchi"))
        {
            potentialTargets.Add(collision.transform);
            UpdateTarget();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (potentialTargets.Contains(collision.transform))
        {
            potentialTargets.Remove(collision.transform);
            UpdateTarget();
        }
    }

    void UpdateTarget()
    {
        if (potentialTargets.Count == 0)
        {
            target = player;  // Default back to player if no gotchis are close
            return;
        }

        Transform closest = null;
        float minDistance = float.MaxValue;
        foreach (Transform t in potentialTargets)
        {
            float dist = Vector2.Distance(transform.position, t.position);
            if (dist < minDistance)
            {
                closest = t;
                minDistance = dist;
            }
        }

        if (closest != null && closest != target)
        {
            target = closest;
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
            attackRoutine = StartCoroutine(DealDamageRepeatedly(target.GetComponent<Collider2D>()));
        }
    }

    IEnumerator DealDamageRepeatedly(Collider2D targetCollider)
    {
        while (!isFrozen && !isDead && targetCollider != null)
        {
            targetCollider.GetComponent<Player>()?.TakeDamage(damageToPlayer);
            targetCollider.GetComponent<WarriorGotchi>()?.TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
        }
    }



    public void TakeDamage(int damage)
    {
        // Ignore damage if already dead
        if (isDead) return;

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
}