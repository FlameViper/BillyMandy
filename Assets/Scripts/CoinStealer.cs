using System.Collections;
using UnityEngine;

public class CoinStealer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;  // Target to move towards initially
    public Transform spawner; // Enemy spawner to return to
    public GameObject coinPrefab;
    public int coinsCollected = 0;

    private float suckDuration = 10f;
    private float suckTimer = 0f;
    private bool returningToSpawner = false;
    private bool isSuckerActive = false;

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
            suckTimer += Time.deltaTime; // Ensure the timer is incremented properly
            Debug.Log("Suck Timer: " + suckTimer);
            if (suckTimer >= suckDuration)
            {
                DepositCoins();
                returningToSpawner = true; // Start returning to spawner
                isSuckerActive = false;
                suckTimer = 0; // Reset timer
                Debug.Log("Finished sucking coins. Returning to spawner.");
            }
        }
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
            isSuckerActive = true;  // Start sucking coins
            Debug.Log("Reached target. Coin sucking activated.");
        }
    }

    private void AttractCoins()
    {
        // Find all game objects tagged as "coin"
        GameObject[] coins = GameObject.FindGameObjectsWithTag("coin");
        Debug.Log("Found " + coins.Length + " coins to potentially attract.");

        foreach (GameObject coin in coins)
        {
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                if (isSuckerActive)
                {
                    // Calculate direction to move the coin towards the Coin Sucker
                    Vector2 directionToSucker = (transform.position - coin.transform.position).normalized;
                    // Apply velocity based on the calculated direction and the move speed
                    coinRb.velocity = directionToSucker * moveSpeed;
                    Debug.Log("Applying force to coin at " + coin.transform.position + " towards Coin Sucker at " + transform.position + " with speed " + moveSpeed);
                }
                else
                {
                    // Stop coin's movement if the sucker is not active
                    coinRb.velocity = Vector2.zero;
                    Debug.Log("Coin Sucker not active, setting velocity of coin at " + coin.transform.position + " to zero.");
                }
            }
            else
            {
                Debug.Log("No Rigidbody2D found on coin at " + coin.transform.position + ", cannot apply force.");
            }
        }
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
