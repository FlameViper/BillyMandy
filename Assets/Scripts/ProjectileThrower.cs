using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ProjectileThrower : MonoBehaviour
{

    public static ProjectileThrower Instance;
    public ProjectileType projectileType = ProjectileType.Fireball;
    public List<GameObject> projectilePrefabs; // Prefab of the projectile to be thrown
    public GameObject coloredProjectilePrefab; // Prefab of the projectile to be thrown
    public List<Sprite> coloredProjectileSprites; // Prefab of the projectile to be thrown
    public List<Color> colorList; // Prefab of the projectile to be thrown
    public float deafaultAttackSpeed = 5f; // Attack speed, measured in attacks per second
    public float coloredProjectileAttackSpeed = 5f; // Attack speed, measured in attacks per second
   
    public bool isThrowerActive = true; // Flag to enable or disable throwing
    public AudioSource attackSound; // AudioSource component for playing attack sounds

    private float lastAttackTime = 0f; // When the last attack happened
    private float lastAttackTimeColored = 0f; // When the last attack happened
    private int currentColoredProjectileTypeIndex;
    private int currentColoredProjectileModeIndex;
   
    //one instance projectiles
    private Projectile projectileInstance;
    private bool hasOneInstance;
    [SerializeField] SpriteRenderer selectedColor;
    [SerializeField] SpriteRenderer color1;
    [SerializeField] SpriteRenderer color2;
    [SerializeField] SpriteRenderer color3;

    private void Awake() {
        if (Instance == null) {

            Instance = this;
        }
    }
    private void Start() {
       
    }

    void Update()
    {
        if (TowerDefenseManager.Instance.isInPreparationPhase) {
            return;
        }
        if (isThrowerActive && Input.GetKeyDown(KeyCode.Alpha1)) {
            currentColoredProjectileModeIndex = (currentColoredProjectileModeIndex + 1) % 3;
        }

        if (isThrowerActive && Input.GetKeyDown(KeyCode.Alpha2)) {
            currentColoredProjectileTypeIndex = (currentColoredProjectileTypeIndex + 1) % 4;
            UpdateProjectileVisuals();
        }

        if (isThrowerActive && Input.GetMouseButtonDown(0) && Time.time - lastAttackTimeColored >= 1f / coloredProjectileAttackSpeed) {
            // Record the time of this attack
            lastAttackTimeColored = Time.time;

            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ensure the z-coordinate is 0 for 2D
            // Calculate the direction towards the mouse position
            Vector3 direction = (mousePosition - transform.position).normalized;
            Vector2 coloredPosition1 = new Vector2(transform.position.x + 0.2f, transform.position.y);

            if (currentColoredProjectileModeIndex == 2) {
                ColoredProjectile coloredProjectile1 = Instantiate(coloredProjectilePrefab, coloredPosition1, Quaternion.identity).GetComponent<ColoredProjectile>();
                coloredProjectile1.spriteRenderer.sprite = coloredProjectileSprites[currentColoredProjectileTypeIndex];
                coloredProjectile1.color = GetColor();
                coloredProjectile1.SetDirection(direction);

            }
            else if (currentColoredProjectileModeIndex == 1) {

                ColoredProjectile coloredProjectile2 = Instantiate(coloredProjectilePrefab, transform.position, Quaternion.identity).GetComponent<ColoredProjectile>();
                coloredProjectile2.spriteRenderer.sprite = coloredProjectileSprites[currentColoredProjectileTypeIndex];
                coloredProjectile2.color = GetColor();
                coloredProjectile2.SetDirection(direction);

            }
        }

        // Check if the thrower is active, for left mouse button click, and if enough time has passed since the last attack
        if (isThrowerActive && ( Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) && projectileType == ProjectileType.Minigun ) && Time.time - lastAttackTime >= 1f / deafaultAttackSpeed)
        {
            if(currentColoredProjectileModeIndex == 1) {
                return;
            }
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
            //if (attackSound != null && !GameSettings.Instance.SFXOFF)
            //{
               // attackSound.Play();
           // }

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


    private string GetColor() {
        switch (currentColoredProjectileTypeIndex) {
            case 0:
                return "Red";

            case 1:
                return "Green";

            case 2:
                return "Yellow";

            case 3:
                return "Purple";

            default:
                return "Red";
        }
    }

    private void UpdateProjectileVisuals() {
        //selectedColor.sprite = coloredProjectileSprites[currentColoredProjectileTypeIndex];
        selectedColor.color = colorList[currentColoredProjectileTypeIndex];

        // Update other colors/sprites as needed
       // color1.sprite = coloredProjectileSprites[(currentColoredProjectileTypeIndex + 1) % 4];
        color1.color = colorList[(currentColoredProjectileTypeIndex + 1) % 4];

       // color2.sprite = coloredProjectileSprites[(currentColoredProjectileTypeIndex + 2) % 4];
        color2.color = colorList[(currentColoredProjectileTypeIndex + 2) % 4];

        //color3.sprite = coloredProjectileSprites[(currentColoredProjectileTypeIndex + 3) % 4];
        color3.color = colorList[(currentColoredProjectileTypeIndex + 3) % 4];
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