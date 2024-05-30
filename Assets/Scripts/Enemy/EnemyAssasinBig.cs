using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAssasinBig : Enemy {



    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float shootInterval = 3f;
    private float shootTimer = 0f;
    private bool isInAttackRange = false;



    protected override void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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

 


}
