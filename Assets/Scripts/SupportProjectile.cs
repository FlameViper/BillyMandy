using System.Collections;
using UnityEngine;

public class SupportProjectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public static float freezeDuration = 2f; // Static freezeDuration to allow global access and modification
    public float destroyDelay = 10f; // Delay before destroying the projectile

    private Vector3 direction; // Direction in which the projectile will move

    void Start()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                StartCoroutine(FreezeEnemy(enemy));
            }

            EnemyShooter enemyShooter = other.GetComponent<EnemyShooter>();
            if (enemyShooter != null)
            {
                StartCoroutine(FreezeEnemyShooter(enemyShooter));
            }

            //Destroy(gameObject); // Destroy the projectile after applying the effect
        }
    }

    IEnumerator FreezeEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.Freeze(true); // Start freezing
            yield return new WaitForSeconds(SupportProjectile.freezeDuration);
            if (enemy != null) {
                enemy.Freeze(false); // Unfreeze after the duration

            }
        }
    }

    IEnumerator FreezeEnemyShooter(EnemyShooter enemyShooter)
    {
        if (enemyShooter != null)
        {
            enemyShooter.Freeze(true); // Start freezing
            yield return new WaitForSeconds(SupportProjectile.freezeDuration);
            if(enemyShooter != null) {
                enemyShooter.Freeze(false); // Unfreeze after the duration

            }
        }
    }




    // Coroutine to destroy the projectile after a delay
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
