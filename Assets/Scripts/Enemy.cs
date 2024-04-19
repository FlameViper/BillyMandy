using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int baseHealth = 30;
    public int currentHealth;
    public float stopDistance = 1f;
    public GameObject coinPrefab;

    public AudioSource damageSound;
    public AudioSource deathSound;

    [SerializeField]
    private Transform target;  // Private but serialized field, only use in inspector

    private SpriteRenderer spriteRenderer;

    public bool isDead = false;
    public bool isFrozen = false;

    // Public property to change target programmatically
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }


    void Start()
    {
        currentHealth = baseHealth;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Update()
    {
        if (isDead || isFrozen || target == null) return; // Stop all movement if enemy is dead, frozen, or no target

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    public void Freeze(bool freezeStatus)
    {
        isFrozen = freezeStatus;
        spriteRenderer.color = freezeStatus ? Color.blue : Color.white;
    }

    IEnumerator UnfreezeAfterDuration(float duration)
    {
        Debug.LogError("freezing");
        yield return new WaitForSeconds(duration);
        Freeze(false);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"TakeDamage called. Current health: {currentHealth}, Damage: {damage}");

        if (isDead || isFrozen) return; // Ignore damage if already dead or frozen

        currentHealth -= damage;

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

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

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
