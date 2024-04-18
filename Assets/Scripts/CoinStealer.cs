using System.Collections;
using UnityEngine;

public class CoinStealer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;
    public Transform spawner;
    public GameObject coinPrefab;
    public int coinsCollected = 0;
    public float baseAttractionPower = 2f;  // Base sucking power
    public float attractionPower;  // Current sucking power, adjusted per level

    private float suckDuration = 10f;
    private float suckTimer = 0f;
    private bool returningToSpawner = false;
    private bool isSuckerActive = false;

    void Start()
    {
        if (BattleManager.Instance != null)
        {
            UpdateAttractionPower(BattleManager.Instance.level);  // Update power on start using the singleton instance
        }
        else
        {
            Debug.LogError("BattleManager instance not found");
        }
    }


    void Update()
    {
        Debug.Log("Update called. Returning to Spawner: " + returningToSpawner + ", Sucker Active: " + isSuckerActive);
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

    public void UpdateAttractionPower(int level)
    {
        float levelMultiplier = 1.0f + (level - 1) * 0.5f;  // Increase power by 10% each level
        attractionPower = baseAttractionPower * levelMultiplier;
        Debug.Log($"Attraction power updated to: {attractionPower} at level {level}");
    }

    void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        Debug.Log("Moving to target. Distance: " + distance);
        if (distance > 1f)
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
        Debug.Log("CoinStealer died. Dropping coins: " + coinsCollected);
        for (int i = 0; i < coinsCollected; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
