using System.Collections;
using UnityEngine;


public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackSpeed = 1f; // Attacks per second
    public float attackRange = 10f; // Range within which the enemy can shoot

    private Transform player;
    private float lastAttackTime = 0f;
    public bool isFrozen = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isFrozen || player == null) return; // Prevent shooting when frozen or if player is null

        // Shooting logic
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (Time.time - lastAttackTime >= 1f / attackSpeed)
            {
                ShootProjectile();
                lastAttackTime = Time.time;
            }
        }
    }

    void ShootProjectile()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null)
        {
            enemyProjectile.SetDirection(direction);
        }
    }

    public void Freeze(bool freezeStatus)
    {
        isFrozen = freezeStatus;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = freezeStatus ? Color.blue : Color.white;
        }

        if (freezeStatus)
        {
            StartCoroutine(UnfreezeAfterDuration()); // Adjusted call
        }
    }

    IEnumerator UnfreezeAfterDuration()
    {
        yield return new WaitForSeconds(SupportProjectile.freezeDuration);
        Freeze(false); // This automatically unfreezes without needing a duration argument
    }



}
