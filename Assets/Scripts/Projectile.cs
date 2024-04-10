using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public static int damageAmount = 10; // Amount of damage dealt to enemies
    public float destroyDelay = 10f; // Delay before destroying the projectile

    private Vector3 direction; // Direction in which the projectile will move

    void Start()
    {
        // Start the coroutine to destroy the projectile after a delay
        StartCoroutine(DestroyAfterDelay());
    }

    void Update()
    {
        // Move the projectile in its direction
        transform.position += direction * speed * Time.deltaTime;
    }

    // Method to set the direction of the projectile
    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is an enemy
        if (other.CompareTag("Enemy"))
        {
            // Get the Enemy component from the collided object
            Enemy enemy = other.GetComponent<Enemy>();

            // If the enemy component exists, apply damage
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }

            // Destroy the projectile
            Destroy(gameObject);
        }
    }

    // Coroutine to destroy the projectile after a delay
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
