using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for UI elements like Text

public class Player : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the player
    public int currentHealth; // Current health of the player
    public GameObject gameOverScreen; // Drag your Game Over UI GameObject here in the inspector
    public Text currentHealthText; // Drag your current health Text UI component here in the inspector
    public Text maxHealthText; // Drag your max health Text UI component here in the inspector

    void Start()
    {
        currentHealth = maxHealth;
        gameOverScreen.SetActive(false); // Ensure the Game Over screen is hidden at start
        UpdateHealthUI(); // Initial update of health display
    }

    void Update()
    {
        UpdateHealthUI(); // Continuously update health display
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeath()); // Start the coroutine to handle death
        }
    }

    private void UpdateHealthUI()
    {
        if (currentHealthText != null)
            currentHealthText.text = "Health: " + currentHealth.ToString();

        if (maxHealthText != null)
            maxHealthText.text = "Max Health: " + maxHealth.ToString();
    }

    IEnumerator HandleDeath()
    {
        Debug.Log("Player Died!");
        gameOverScreen.SetActive(true); // Show the Game Over screen
        yield return new WaitForSeconds(5); // Wait for 5 seconds
        gameOverScreen.SetActive(false); // Optional: hide Game Over screen before reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the scene
    }
}
