using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy09 : Enemy
{
    [SerializeField] private float rotationSpeed = 60.0f;
    [SerializeField] private float radius = 3.0f;
    [SerializeField] private float angle = 0f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 10f; 
    [SerializeField] private float lastAttackTime = 0f;

    protected override void Start() {
        base.Start();

    }

    protected override void Update() {
        base.Update();
        if (isFrozen) {
            return;
        }
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
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }

        float x = MathF.Cos(angle * Mathf.Deg2Rad) * radius;
        float y = MathF.Sin(angle * Mathf.Deg2Rad) * radius;

        Vector3 circularMotion = new Vector3 (x, y, 0);
        transform.position += circularMotion * Time.deltaTime;

        angle += rotationSpeed * Time.deltaTime;
        if (angle >= 360.0f) {
            angle -= 360.0f;
        }

        if (transform.position.y < -10.55f) {
            float randomX = UnityEngine.Random.Range(-9.50f,9.50f);
            transform.position = new Vector3(randomX, 7.60f, 0);

        }
    }
}
    
