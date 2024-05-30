
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAssasinAssault : Enemy {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private float cricularMovementSpeedMultiplier = 8f;
    private float shootTimer = 0f;
    private bool isInAttackRange = false;
    private float semiCircleAngle = 180f;
    [SerializeField] private bool movingClockwise = true;


    protected override void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        int random = Random.Range(0, 2);
        if (random == 0) {
            semiCircleAngle = 0f;
        }
        else {
            semiCircleAngle = 180f;
        }

    }

    protected override void Update() {
        base.Update();

    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("WarriorGotchi")) {
            potentialTargets.Add(collision.transform);
            UpdateTarget();
        }
    }
    protected override void Movement() {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange) {
            isInAttackRange = true;
        }
        else {
            isInAttackRange = false;
        }

        if (isInAttackRange) {
            MoveInSemiCircle();
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval) {
                ShootProjectile();
                shootTimer = 0f;
            }
        }
        else {
            MoveTowardsPlayer();
        }
    }

    private void ShootProjectile() {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null) {
            enemyProjectile.SetDirection(direction);
        }
    }
    private void MoveTowardsPlayer() {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        
    }

    private void MoveInSemiCircle() {
        if (movingClockwise) {
            semiCircleAngle += moveSpeed * cricularMovementSpeedMultiplier * Time.deltaTime;
            if (semiCircleAngle > 180f) {
                semiCircleAngle = 180f;
                movingClockwise = false;
            }
        }
        else {
            semiCircleAngle -= moveSpeed * cricularMovementSpeedMultiplier * Time.deltaTime;
            if (semiCircleAngle < 0) {
                semiCircleAngle = 0;
                movingClockwise = true;
            }
        }

        Vector3 offset = new Vector3(Mathf.Cos(semiCircleAngle * Mathf.Deg2Rad), Mathf.Sin(semiCircleAngle * Mathf.Deg2Rad), 0) * attackRange;
        transform.position = Vector2.MoveTowards(transform.position, player.position + offset, moveSpeed * Time.deltaTime);
    }



}
