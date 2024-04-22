using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public static int damageAmount = 20; // Amount of damage dealt to enemies
    public float destroyDelay = 10f; // Delay before destroying the projectile

    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform canvasTransform; // Reference to the transform of the Canvas object


    private Vector3 direction; // Direction in which the projectile will move

    void Start()
    {
        if (UIManager.Instance.battleCanvasTransform == null)
        {
            Debug.LogError("BattleCanvas Transform is not set in the UIManager.");
            return;
        }
        canvasTransform = UIManager.Instance.battleCanvasTransform;
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
                DisplayDamage(damageAmount, transform.position);
                Destroy(gameObject);
            }
        }
        // Additional condition for CoinStealer
        else if (other.CompareTag("CoinStealer"))
        {
            // Get the CoinStealer component from the collided object
            CoinStealer coinStealer = other.GetComponent<CoinStealer>();

            // If the CoinStealer component exists, apply damage
            if (coinStealer != null)
            {
                coinStealer.TakeDamage(damageAmount);
                DisplayDamage(damageAmount, transform.position);
                Destroy(gameObject);
            }
        }

    }

    void DisplayDamage(int damage, Vector3 position)
    {
        GameObject damageTextObject = Instantiate(damageTextPrefab, position, Quaternion.identity, canvasTransform);
        Text textComponent = damageTextObject.GetComponent<Text>();
        textComponent.text = damage.ToString("F0");
        // Ensure the text is visible above everything else
        damageTextObject.transform.localPosition = new Vector3(damageTextObject.transform.localPosition.x, damageTextObject.transform.localPosition.y, 0);
    }

    // Coroutine to destroy the projectile after a delay
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
