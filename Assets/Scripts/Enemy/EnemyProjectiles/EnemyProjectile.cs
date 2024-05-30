using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float speed = 10f; // Speed of the projectile
    public int damageAmount = 10; // Amount of damage dealt to the player
    public float destroyDelay = 10f; // Delay before destroying the projectile

    protected Vector3 direction; // Direction in which the projectile will move

    protected virtual void Start()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    protected virtual void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public virtual void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
    }

    protected virtual IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
