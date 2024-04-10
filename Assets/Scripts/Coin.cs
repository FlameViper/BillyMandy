using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // You can adjust the coin value if needed
    public int coinValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Find the ResourceManager in the scene and add coins
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddCoins(coinValue);
            }
            else
            {
                Debug.LogError("ResourceManager not found in the scene. Make sure it exists and is active.");
            }

            // Destroy the coin after giving its value to the player
            Destroy(gameObject);
        }
    }
}
