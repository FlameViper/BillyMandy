using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the player
    public int currentHealth; // Current health of the player

    void Start()
    {
        currentHealth = maxHealth; // Set current health to maximum at start
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce current health by the damage amount

        // Optionally, check if the player's health has dropped to 0 or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle player death here (e.g., restart level, show game over screen)
        Debug.Log("Player Died!");
    }
}
