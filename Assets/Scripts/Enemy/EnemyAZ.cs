using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAZ : Enemy
{
    [SerializeField] private float zigzagFrequency = 2.0f;
    [SerializeField] private float zigzagMagnitude = 1.0f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float lastAttackTime = 0f;

    protected override void Start() {
        base.Start();
       
    }
    protected override void Update() {
        base.Update();
        // Shooting logic
        if (Vector3.Distance(transform.position, player.position) <= attackRange) {
            if (Time.time - lastAttackTime >= 1f / attackSpeed) {
                ShootProjectile();
                lastAttackTime = Time.time;
            }
        }
    }

    void ShootProjectile() {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null) {
            enemyProjectile.SetDirection(direction);
        }
    }

    protected override void Movement() {
        CalculateMovement();

    }


    private void CalculateMovement() {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > stopDistance) {
            // Move towards the player
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
        else {
            return;
        }

        // Calculate direction towards the player
        Vector2 directionToPlayer = (target.position - transform.position).normalized;

        // Calculate perpendicular direction for zigzag motion
        Vector2 perpendicularDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x);

        // Calculate zigzag motion
        float zigzagAmount = Mathf.Sin(Time.time * zigzagFrequency) * zigzagMagnitude;

        // Move in zigzag pattern while going towards the player
        Vector2 zigzagMotion = perpendicularDirection * zigzagAmount;
        transform.position += (Vector3)(directionToPlayer * moveSpeed + zigzagMotion) * Time.deltaTime;

        if (transform.position.y < -10.55f) {
            float randomX = UnityEngine.Random.Range(-9.50f, 9.50f);
            transform.position = new Vector3(randomX, 7.60f, 0);
        }
    }


}
