using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAssasinTower : Enemy {


    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject bigprojectilePrefab;
    [SerializeField] private float deployRange = 6f;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private float shootBigInterval = 4f;
    [SerializeField] private float deployTime = 2f;
    [SerializeField] private float deploySpeed = 1f;
    [SerializeField] private GameObject wing1;
    [SerializeField] private GameObject wing2;
    [SerializeField] private Vector2 wing1Position;
    [SerializeField] private Vector2 wing2Position;


    private float shootTimer = 0f;
    private float shootBigTimer = 0f;
    private bool isInAttackRange = false;
    private bool isDeployed = false;

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

        if (distanceToPlayer <= deployRange) {
            if (!isInAttackRange) {
                isInAttackRange = true;
                StartCoroutine(DeployAndAttack());
            }
        }
        else {
            isInAttackRange = false;
            isDeployed = false;
            StopCoroutine(DeployAndAttack());
            MoveTowardsPlayer();
        }
    }

    private IEnumerator DeployAndAttack() {
        Vector2 initialWing1Position = wing1.transform.localPosition;
        Vector2 initialWing2Position = wing2.transform.localPosition;

        float elapsedTime = 0f;
        while (elapsedTime < deployTime) {
            wing1.transform.localPosition = Vector2.Lerp(initialWing1Position, wing1Position, elapsedTime / deployTime);
            wing2.transform.localPosition = Vector2.Lerp(initialWing2Position, wing2Position, elapsedTime / deployTime);
            elapsedTime += Time.deltaTime * deploySpeed;
            yield return null;
        }

        wing1.transform.localPosition = wing1Position;
        wing2.transform.localPosition = wing2Position;

        isDeployed = true;

        while (isInAttackRange) {
            shootBigTimer += Time.deltaTime;
            if (shootBigTimer >= shootBigInterval) {
                ShootBigProjectile();
                shootBigTimer = 0f;
            }

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval) {
                ShootProjectile();
                shootTimer = 0f;
            }

            yield return null;
        }
    }
    private void ShootProjectile() {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile1 = Instantiate(projectilePrefab, new Vector3(transform.position.x +0.3f,transform.position.y), Quaternion.LookRotation(Vector3.forward, direction));
        GameObject projectile2 = Instantiate(projectilePrefab, new Vector3(transform.position.x - 0.3f, transform.position.y), Quaternion.LookRotation(Vector3.forward, direction));
        EnemyProjectile enemyProjectile1 = projectile1.GetComponent<EnemyProjectile>();
        EnemyProjectile enemyProjectile2 = projectile2.GetComponent<EnemyProjectile>();
        if (enemyProjectile1 != null) {
            enemyProjectile1.SetDirection(direction);
        }
        if (enemyProjectile2 != null) {
            enemyProjectile2.SetDirection(direction);
        }
    }
    private void ShootBigProjectile() {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(bigprojectilePrefab, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null) {
            enemyProjectile.SetDirection(direction);
        }
    }
    private void MoveTowardsPlayer() {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

    }


    public override void Freeze(bool solidFreeze) {


    }
}
