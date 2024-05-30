using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAssasinBungee : Enemy {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private int shotsPerAttack = 2; 
    [SerializeField] private float returnDelay = 1f; 
    private Vector3 originalPosition;
    private Coroutine returnToOriginalPositionCorroutine;
    private Vector3 randomPosition;
    private bool isReturning = false;
 
    private int shotsFired = 0;
    private float lastAttackTime = 0f;


    protected override void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalPosition = transform.position;
        PickRandomPosition(player.position);
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
        if (!isReturning) {
            MoveToRandomPosition();
            //if(Vector3.Distance(transform.position, randomPosition) <= 0.5) {
            //    Attack();
            //}
            if (Vector3.Distance(transform.position, player.position) <= attackRange) {
                Attack();
            }

        }
        else {
            ReturnToOriginalPosition();
            if (Vector3.Distance(transform.position, player.position) <= attackRange) {
                Attack();
            }
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

 
    private void MoveToRandomPosition() {
        //// Move towards a random position around the player
        transform.position = Vector3.MoveTowards(transform.position, randomPosition, moveSpeed * Time.deltaTime);
  
    }

    private void PickRandomPosition(Vector3 targetPosition) {
        float randomRadius = UnityEngine.Random.Range(3, attackRange - 1f);
        Vector2 randomDirection = (UnityEngine.Random.insideUnitCircle).normalized;
        Vector2 randomOffset = randomDirection * randomRadius;

        // Ensure the y-coordinate is at least 3 units higher than the target position's y-coordinate
        float newY = targetPosition.y + 3;
        if (newY <= targetPosition.y + 3) {
            newY = targetPosition.y + 3;
        }

        randomPosition = targetPosition + new Vector3(randomOffset.x, newY - targetPosition.y, 0f);
    }

    private void PickRandomReturnPosition(Vector3 targetPosition) {
        float randomRadius = UnityEngine.Random.Range(attackRange, attackRange * 2);
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * randomRadius;
        randomPosition = targetPosition + new Vector3(randomOffset.x, 0, 0f);

    }
    private void Attack() {

        if (Time.time - lastAttackTime >= 1f / attackSpeed) {
            ShootProjectile();
            lastAttackTime = Time.time;
            shotsFired++;
            // Check if reached the maximum shots per attack
            if (shotsFired >= shotsPerAttack) {
                isReturning = true;
                shotsFired = 0;
            }
        }
        
    }


    private IEnumerator ReturnToOriginalPositionCoroutine() {
        // Calculate the target position based on the original position
        PickRandomReturnPosition(originalPosition);
        Vector3 targetPosition = randomPosition;

        // Move towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 1) {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            yield return null; // Wait for the next frame
        }

        // Wait for the specified delay
        yield return new WaitForSeconds(returnDelay);

        // After the delay, pick a new random position and set isReturning to false
        PickRandomPosition(player.position);
        isReturning = false;
        returnToOriginalPositionCorroutine = null;
    }

    private void ReturnToOriginalPosition() {
        // Start the coroutine only if it's not already running
        if (returnToOriginalPositionCorroutine == null) {
            returnToOriginalPositionCorroutine = StartCoroutine(ReturnToOriginalPositionCoroutine());
        }
    }

}
