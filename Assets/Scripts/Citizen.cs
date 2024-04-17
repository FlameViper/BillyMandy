using UnityEngine;

public class Citizen : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Vector2 boundaryMin;
    public Vector2 boundaryMax;
    public GameObject projectilePrefab;  // Reference to the projectile prefab
    public float baseShootingInterval = 2.0f;  // Base time between shots

    private Vector2 targetPosition;
    private float lastShotTime = 0;  // Track the last time a shot was fired
    private float nextShotTime;      // Time until the next shot

    void Start()
    {
        SetRandomTargetPosition();
        SetNextShotTime();
    }

    void Update()
    {
        MoveTowardsTarget();
        ShootProjectile();
    }

    private void MoveTowardsTarget()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the target position has been reached
        if ((Vector2)transform.position == targetPosition)
        {
            SetRandomTargetPosition();
        }
    }

    private void SetRandomTargetPosition()
    {
        // Generate a random position within the boundaries
        float randomX = Random.Range(boundaryMin.x, boundaryMax.x);
        float randomY = Random.Range(boundaryMin.y, boundaryMax.y);
        targetPosition = new Vector2(randomX, randomY);
    }

    private void ShootProjectile()
    {
        // Check if enough time has passed since the last shot
        if (Time.time - lastShotTime >= nextShotTime)
        {
            lastShotTime = Time.time;  // Update last shot time
            SetNextShotTime();         // Set the time for the next shot

            // Create a projectile and set it to move upwards
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = newProjectile.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetDirection(Vector2.up);  // Set the projectile to move upwards in 2D
            }
            else
            {
                Debug.LogError("Projectile prefab doesn't have Projectile component attached!");
            }
        }
    }

    private void SetNextShotTime()
    {
        // Randomize the next shot time within +/- 10% of the base interval
        nextShotTime = baseShootingInterval * Random.Range(0.8f, 1.2f);
    }
}
