using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    private Rigidbody2D rb;
    private Dictionary<GameObject, float> attractors = new Dictionary<GameObject, float>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (attractors.Count > 0)
        {
            Vector2 totalForce = Vector2.zero;
            foreach (var attractor in attractors)
            {
                if (attractor.Key != null)  // Ensure the attractor is still in the scene
                {
                    Vector2 direction = (attractor.Key.transform.position - transform.position).normalized;
                    totalForce += direction * attractor.Value;
                }
            }

            // Apply force only if there is a significant total force calculated
            if (totalForce.magnitude > 0.01f)
                rb.velocity = totalForce;
            else
                rb.velocity = Vector2.zero;
        }
        else
        {
            // If no attractors are present, stop moving the coin
            rb.velocity = Vector2.zero;
        }
    }

    public void AddAttractor(GameObject attractor, float power)
    {
        if (attractors.ContainsKey(attractor))
        {
            attractors[attractor] = power;
        }
        else
        {
            attractors.Add(attractor, power);
        }
    }

    public void RemoveAttractor(GameObject attractor)
    {
        if (attractors.ContainsKey(attractor))
        {
            attractors.Remove(attractor);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("CoinStealer"))
        {
            HandleCollection(other);
        }
    }

    private void HandleCollection(Collider2D collector)
    {
        if (collector.CompareTag("Player"))
        {
            AddCoinValueToResourceManager();
        }
        else if (collector.CompareTag("CoinStealer"))
        {
            CoinStealer stealer = collector.GetComponent<CoinStealer>();
            if (stealer != null)
            {
                stealer.coinsCollected += coinValue;
            }
        }
        Destroy(gameObject); // Destroy the coin after it is collected
    }

    private void AddCoinValueToResourceManager()
    {
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddCoins(coinValue);
        }
        else
        {
            Debug.LogError("ResourceManager not found in the scene. Make sure it exists and is active.");
        }
    }
}
