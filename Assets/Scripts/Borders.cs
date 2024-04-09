using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borders : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider that entered the borders is tagged as "Projectile"
        if (other.CompareTag("Projectile"))
        {
            // Destroy the projectile
            Destroy(other.gameObject);
            
        }
    }
}

