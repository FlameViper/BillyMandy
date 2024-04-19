using System.Collections;
using UnityEngine;

public class CoinStealer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform target;  // Will be assigned dynamically
    public Transform spawner;  // Will be assigned dynamically
    public GameObject coinPrefab;
    public int coinsCollected = 0;
    public float baseAttractionPower = 1f;  // Base sucking power
    public float attractionPower;  // Current sucking power, adjusted per level
    public int baseHealth = 200;  // Base health
    public int currentHealth;  // Current health

    private float suckDuration = 10f;
    private float suckTimer = 0f;
    private bool returningToSpawner = false;
    private bool isSuckerActive = false;
    private bool isDead = false;  // Flag to check if the CoinStealer is dead

    private SpriteRenderer spriteRenderer; // Make sure to initialize this in Start or Awake

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the CoinStealer.");
        }
    }

    void Start()
    {
        target = GameObject.Find("CoinStealerLocation").transform;  // Find the target location by name
        spawner = GameObject.Find("EnemySpawner").transform;  // Find the spawner by name

        currentHealth = baseHealth;  // Initialize health
        if (BattleManager.Instance != null)
        {
            UpdateAttractionPower(BattleManager.Instance.level);  // Update power on start
            SetHealth(BattleManager.Instance.level);  // Update health based on level
        }
        else
        {
            Debug.LogError("BattleManager instance not found");
        }
    }


    void Update()
    {
        if (!returningToSpawner)
        {
            MoveToTarget();
        }
        else
        {
            ReturnToSpawner();
        }

        if (isSuckerActive)
        {
            AttractCoins();
            suckTimer += Time.deltaTime;
            if (suckTimer >= suckDuration)
            {
                StopSucking();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;  // Ignore damage if already dead

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void UpdateAttractionPower(int level)
    {
        float levelMultiplier = 1.0f + (level - 1) * 0.5f;  // Increase power by 10% each level
        attractionPower = baseAttractionPower * levelMultiplier;
        Debug.Log($"Attraction power updated to: {attractionPower} at level {level}");
    }

    void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > 0.3f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
        else if (!isSuckerActive)
        {
            StartSucking();
        }
    }

    private void AttractCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("coin");
        Debug.Log("Found " + coins.Length + " coins to potentially attract.");

        foreach (GameObject coin in coins)
        {
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null && isSuckerActive)
            {
                coinScript.AddAttractor(gameObject, attractionPower);
            }
        }
    }

    private void StartSucking()
    {
        isSuckerActive = true;
        Debug.Log("Coin sucking activated.");
    }

    private void StopSucking()
    {
        isSuckerActive = false;
        suckTimer = 0;
        Debug.Log("Finished sucking coins. Returning to spawner.");
        RemoveAsAttractor(); // Ensure to remove as attractor when stops sucking
        returningToSpawner = true;
    }

    private void RemoveAsAttractor()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("coin");
        foreach (GameObject coin in coins)
        {
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.RemoveAttractor(gameObject);
            }
        }
    }

    private void OnDisable()
    {
        StopSucking(); // Also handle removing as attractor when disabled
    }

    void ReturnToSpawner()
    {
        float distance = Vector2.Distance(transform.position, spawner.position);
        Debug.Log("Returning to spawner. Distance: " + distance);
        if (distance > 1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, spawner.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            DepositCoins();
            Debug.Log("Deposited coins: " + coinsCollected);
            Destroy(gameObject);  // Destroy or disable after depositing coins
        }
    }

    void DepositCoins()
    {
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddEnemyCoins(coinsCollected);
            Debug.Log("Depositing coins to resource manager: " + coinsCollected);
        }
        else
        {
            Debug.Log("Resource Manager not found.");
        }
        coinsCollected = 0;  // Reset coin count after deposit
    }

    public void OnDeath()
    {
        if (isDead) return; // Prevent multiple deaths
        isDead = true; // Mark as dead

        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddScore(300);
        }

        StartCoroutine(FadeOutAndDropCoins(1f)); // Start fading out with a duration of 1 second
    }

    IEnumerator FadeOutAndDropCoins(float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }

        Debug.Log("CoinStealer died. Dropping coins: " + coinsCollected);
        float dropRadius = 1f; // Define the radius within which coins will be dropped
        for (int i = 0; i < coinsCollected; i++)
        {
            Vector3 coinPosition = transform.position + new Vector3(Random.Range(-dropRadius, dropRadius), Random.Range(-dropRadius, dropRadius), 0);
            Instantiate(coinPrefab, coinPosition, Quaternion.identity);
        }
        Destroy(gameObject); // Destroy the object after fading out
    }

    public void SetHealth(int level)
    {
        int healthIncrease = 50; // Default for Easy

        switch (GameSettings.Instance.currentDifficulty)
        {
            case GameSettings.Difficulty.Medium:
                healthIncrease = 70;
                break;
            case GameSettings.Difficulty.Hard:
                healthIncrease = 100;
                break;
        }

        baseHealth = 200 + (level - 1) * healthIncrease;
        currentHealth = baseHealth;
    }
}
