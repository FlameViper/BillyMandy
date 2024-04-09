using UnityEngine;

public class CoinSucker : MonoBehaviour
{
    public bool isSuckerActive = false;
    public float moveSpeed = 5f; // The constant speed at which coins will move towards the Coin Sucker

    private void Update()
    {
        AttractCoins();
    }

    private void AttractCoins()
    {
        // Find all game objects tagged as "coin"
        GameObject[] coins = GameObject.FindGameObjectsWithTag("coin");

        foreach (GameObject coin in coins)
        {
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                if (isSuckerActive)
                {
                    // Move coin towards the Coin Sucker at a constant speed
                    Vector2 directionToSucker = (transform.position - coin.transform.position).normalized;
                    coinRb.velocity = directionToSucker * moveSpeed;
                }
                else
                {
                    // Optionally stop the coin's movement when the Coin Sucker is not active
                    coinRb.velocity = Vector2.zero;
                }
            }
        }
    }

    public void ToggleSucker()
    {
        isSuckerActive = !isSuckerActive;
    }
}
