using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the player
    public int currentHealth; // Current health of the player
    public GameObject gameOverScreen; // Drag your Game Over UI GameObject here in the inspector
    public Text healthText; // Drag your health Text UI component here in the inspector
    public AudioSource deathSound; // Reference to the AudioSource component that plays the death sound
    private bool isDead = false; // Flag to prevent multiple death sequences

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
        if (isDead) return; // If already dead, do nothing

        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true; // Set isDead to true to prevent multiple calls
            deathSound.Play(); // Play the death sound
            StartCoroutine(HandleDeath()); // Start the coroutine to handle death
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "HP " + currentHealth + "/" + maxHealth; // Displays as 'currentHealth/maxHealth', e.g., '50/100'
    }

    IEnumerator HandleDeath()
    {
        Debug.Log("Player Died!");
        gameOverScreen.SetActive(true); // Show the Game Over screen
        yield return new WaitForSeconds(5); // Wait for 5 seconds
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the scene
    }
}
