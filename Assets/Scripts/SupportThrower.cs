using UnityEngine;

public class SupportThrower : MonoBehaviour
{
    public GameObject supportProjectilePrefab; // Prefab of the support projectile to be thrown
    public float supportAttackSpeed = 1f; // Attack speed, measured in attacks per second
    public bool isSupportActive = true; // Flag to enable or disable support throwing

    private float lastSupportAttackTime = 0f; // When the last support attack happened

    void Update()
    {
        if (isSupportActive && Input.GetMouseButtonDown(0) && Time.time - lastSupportAttackTime >= 1f / supportAttackSpeed)
        {
            Debug.Log("Attempting to throw support projectile");
            lastSupportAttackTime = Time.time;

            // Get the mouse position in world coordinates and adjust for 2D
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            // Correctly calculate the direction towards the mouse position before using it
            Vector3 direction = (mousePosition - transform.position).normalized;

            GameObject newSupportProjectile = Instantiate(supportProjectilePrefab, transform.position, Quaternion.identity);
            Debug.Log($"Support projectile instantiated at {transform.position}");

            Projectile supportProjectile = newSupportProjectile.GetComponent<Projectile>();

            if (supportProjectile != null)
            {
                supportProjectile.SetDirection(direction);
                Debug.Log("Support projectile direction set");
            }
            else
            {
                Debug.LogError("Support projectile prefab doesn't have Projectile component attached!");
            }
        }
    }
}
