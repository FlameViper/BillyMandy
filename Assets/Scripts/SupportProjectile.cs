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
                enemy.Freeze(true); // Assuming Freeze() now accesses SupportProjectile.freezeDuration internally
            }

            EnemyShooter enemyShooter = other.GetComponent<EnemyShooter>();
            if (enemyShooter != null)
            {
                enemyShooter.Freeze(true); // Similarly adjusted for EnemyShooter
            }

           // Destroy(gameObject); // Destroy the projectile after applying the effect
        }
    }



    IEnumerator FreezeEnemy(Enemy enemy)
    {
        enemy.Freeze(true); // Now simply passing the freeze status
        yield return new WaitForSeconds(SupportProjectile.freezeDuration);
        enemy.Freeze(false);
    }

    IEnumerator FreezeEnemyShooter(EnemyShooter enemyShooter)
    {
        enemyShooter.Freeze(true); // Again, only passing the freeze status
        yield return new WaitForSeconds(SupportProjectile.freezeDuration);
        enemyShooter.Freeze(false);
    }


    // Coroutine to destroy the projectile after a delay
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
