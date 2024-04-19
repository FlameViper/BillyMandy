using System.Collections;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public int damageToPlayer = 10;
    public float attackSpeed = 1f;
    private Coroutine attackRoutine = null;

    private Enemy enemyComponent; // Reference to the Enemy component

    void Start()
    {
        enemyComponent = GetComponent<Enemy>(); // Get the Enemy component from the same GameObject
        if (enemyComponent == null)
        {
            Debug.LogError("EnemyMelee cannot find an Enemy component on the GameObject.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only attack if the collision object has the Player tag, there's no ongoing attack, and the enemy is not dead
        if (collision.CompareTag("Player") && attackRoutine == null && !enemyComponent.isDead)
        {
            attackRoutine = StartCoroutine(DealDamageRepeatedly(collision));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Stop the attack when the player leaves the trigger zone
        if (collision.CompareTag("Player") && attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    IEnumerator DealDamageRepeatedly(Collider2D playerCollider)
    {
        // Continuously deal damage to the player while within range and not disabled
        while (!enemyComponent.isFrozen && !enemyComponent.isDead)
        {
            playerCollider.GetComponent<Player>().TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
        }
    }
}
