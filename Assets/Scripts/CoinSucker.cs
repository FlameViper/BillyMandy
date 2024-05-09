using UnityEngine;

public class CoinSucker : MonoBehaviour
{
    public static CoinSucker Instance; 
    public bool isSuckerActive = false;
    public float SuckPower = 5f;
    private void Awake() {
        if (Instance == null) {

            Instance = this;
        }
    }
    void Update()
    {
        // Constantly check if the sucker should be active and manage coin attraction accordingly
        if (isSuckerActive)
        {
            AttractCoins();
        }
        else
        {
            RemoveAsAttractor();  // Ensure no coins are being attracted when it's not active
        }
    }

    private void AttractCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("coin");
        foreach (GameObject coin in coins)
        {
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.AddAttractor(gameObject, SuckPower);
            }
        }
    }

    private void OnDisable()
    {
        // Ensure to remove this object as an attractor when it is disabled or destroyed
        RemoveAsAttractor();
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

    public void ToggleSucker()
    {
        isSuckerActive = !isSuckerActive;
        // When toggling, manage the adding/removing from attractors appropriately
        if (isSuckerActive)
        {
            // No need to call AttractCoins here as it will be called in Update if active
        }
        else
        {
            RemoveAsAttractor();  // Stop attracting coins when deactivated
        }
    }


   
}
