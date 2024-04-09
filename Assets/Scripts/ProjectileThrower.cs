using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab of the projectile to be thrown
    public float attackSpeed = 1f; // Attack speed, measured in attacks per second
    public bool isThrowerActive = true; // Flag to enable or disable throwing

    private float lastAttackTime = 0f; // When the last attack happened

    void Update()
    {
        // Check if the thrower is active, for left mouse button click, and if enough time has passed since the last attack
        if (isThrowerActive && Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= 1f / attackSpeed)
        {
            // Record the time of this attack
            lastAttackTime = Time.time;

            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ensure the z-coordinate is 0 for 2D

            // Calculate the direction towards the mouse position
            Vector3 direction = (mousePosition - transform.position).normalized;

            // Instantiate the projectile at the current position
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Get the Projectile component from the instantiated projectile
            Projectile projectile = newProjectile.GetComponent<Projectile>();

            // If the projectile has a Projectile component, set its direction
            if (projectile != null)
            {
                projectile.SetDirection(direction);
            }
            else
            {
                Debug.LogError("Projectile prefab doesn't have Projectile component attached!");
            }
        }
    }
}
