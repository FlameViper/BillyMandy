using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{

    public static ProjectileThrower Instance;
    public ProjectileType projectileType = ProjectileType.Fireball;
    public List<GameObject> projectilePrefabs; // Prefab of the projectile to be thrown
    public float deafaultAttackSpeed = 5f; // Attack speed, measured in attacks per second
   
    public bool isThrowerActive = true; // Flag to enable or disable throwing
    public AudioSource attackSound; // AudioSource component for playing attack sounds

    private float lastAttackTime = 0f; // When the last attack happened

   
    //one instance projectiles
    private Projectile projectileInstance;
    private bool hasOneInstance;
   

    private void Awake() {
        if (Instance == null) {

            Instance = this;
        }
    }
    private void Start() {
       
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {

        }
        // Check if the thrower is active, for left mouse button click, and if enough time has passed since the last attack
        if (isThrowerActive && ( Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) && projectileType == ProjectileType.Minigun ) && Time.time - lastAttackTime >= 1f / deafaultAttackSpeed)
        {
            // Record the time of this attack
            lastAttackTime = Time.time;

            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ensure the z-coordinate is 0 for 2D

            // Calculate the direction towards the mouse position
            Vector3 direction = (mousePosition - transform.position).normalized;

            if(hasOneInstance) {
                if (projectileInstance != null) {
                    return;
                }
            }

            // Instantiate the projectile at the current position
            GameObject newProjectile = InstantiateProjectile(projectileType);

            // Get the Projectile component from the instantiated projectile
            //Projectile projectile = newProjectile.GetComponent<Projectile>();
            projectileInstance = newProjectile.GetComponent<Projectile>();
 
            // If the projectile has a Projectile component, set its direction
            if (projectileInstance != null)
            {
                if (projectileType != ProjectileType.Balistic) {
                    projectileInstance.SetDirection(direction);
                }
                else {
                    projectileInstance.SetDirection(mousePosition);
                }
               
            }
            else
            {
                Debug.LogError("Projectile prefab doesn't have Projectile component attached!");
            }

            // Play the attack sound if the AudioSource and clip are available
            if (attackSound != null)
            {
                attackSound.Play();
            }
            else
            {
                Debug.LogError("No AudioSource component found on the GameObject!");
            }
        }


    }

    private GameObject InstantiateProjectile(ProjectileType projectileType) {
        switch (projectileType) {
            case ProjectileType.Fireball:
                hasOneInstance = false;
                return Instantiate(projectilePrefabs[0], transform.position, Quaternion.identity);
            case ProjectileType.Boomerang:
                hasOneInstance = true;
                return Instantiate(projectilePrefabs[1], transform.position, Quaternion.identity);
            case ProjectileType.Balistic:
                hasOneInstance = false;
                return Instantiate(projectilePrefabs[2], transform.position, Quaternion.identity);
            case ProjectileType.BlueFire:
                hasOneInstance = false;
                return Instantiate(projectilePrefabs[3], transform.position, Quaternion.identity);
            case ProjectileType.Minigun:
                hasOneInstance = false;
                return Instantiate(projectilePrefabs[4], transform.position, Quaternion.identity);
            case ProjectileType.Lazer:
                hasOneInstance = true;
                return Instantiate(projectilePrefabs[5], transform.position, Quaternion.identity);
            default:
                // Handle unknown projectile types or return null
                Debug.Log("Projectile is not assigned");
                return null;
        }
    }

}
public enum ProjectileType {
    Fireball,
    Boomerang,
    Balistic,
    BlueFire,
    Lazer,
    Minigun,
 
}