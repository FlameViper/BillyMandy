using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GoldenSnitchBoss : Enemy {

    public static GoldenSnitchBoss Instance;

    public event EventHandler OnExplosionTriggerd;

    [SerializeField] private bool takesObjectiveDamage = true;
    [SerializeField] private int objectiveOnHitDamageTaken=10;
    [SerializeField] private int reflectedDmgToPlayer=5;
    [SerializeField] private GameObject smallProjectilePrefab;
    [SerializeField] private GameObject bigPlasmaProjectilePrefab;
    [SerializeField] private float smallRangeAttackInterval=1f;
    [SerializeField] private int smallRangeAttackProjectileNumber=3;
    [SerializeField] private float bigRangeAttackColldown = 30f;
    private Vector2 bigrangeAttackPosition = new Vector2(0,3.5f);
    public bool phase1Active=true;
    public bool phase2Active;
    public bool shield1Active;
    public bool shield2Active;
    public bool isMoving = true;
    public bool shieldOnCooldonwn;
    public bool performingMeleeAttack;
    public bool gettingPushed;
    public bool canBigRangeAttack;
    public bool performingBigRangeAttack;
    public bool explosionJustTriggered;
    public Coroutine moveToEnemyPathCoroutine;
    private Vector2 hoverAbovePlayerPosition = new Vector2(0,0);
    [SerializeField] private float meleeAttackHoverTime=5f;
    [SerializeField] private float meleeAttackDmgInterval=1f;
    [SerializeField] private int meleeAttackDamage = 5;
    private List<Vector2> currentEnemyPath = new List<Vector2>();
    [SerializeField] private List<Vector2> phase1MovementLocations;
    [SerializeField] private List<Vector2> phase2MovementLocations;
    [SerializeField] private List<string> phase1MovementPatterns;
    [SerializeField] private List<string> phase2MovementPatterns;
    [SerializeField] private GameObject shield1;
    [SerializeField] private GameObject shield2;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private int shield1MaxHp=500;
    [SerializeField] private int shield2MaxHp=250;
    [SerializeField] private float shieldResetCooldown;
    [SerializeField] private int shieldCurrentHp;
    [SerializeField] private Collider2D shieldCollider;
    private Coroutine triggerShieldCoroutine;
    private Coroutine triggerShieldCooldownCoroutine;
    private Coroutine checkForCoinSukerCoroutine;
    private Coroutine getPushedAwayCoroutine;
    private Coroutine randomRangeShootingCoroutine;
    private Coroutine bigRangeAttackCoroutine;
    private Coroutine bigRangeAttackCooldownCoroutine;
    private Coroutine explosionJustTriggeredCortuine;
    private int movement1Count;
    private int movement2Count;
    //rotation
    [SerializeField] private Transform rotateAroundPoint;
    [SerializeField] private float rotationSpeed = 60.0f;
    [SerializeField] private float rotationDuration = 6f;
    [SerializeField] private float pointMoveSpeed = 1f;   
    [SerializeField] private float pointMoveRange = 5f; 
    [SerializeField] private float radius = 3f;
    BossBigProjectile bossBigProjectile;

    private void Awake() {
        if (Instance == null) {

            Instance = this;
        }
    }

    protected override void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        baseColor = spriteRenderer.color;
    }

    protected override void Update() {
        if (isDead) {
            return;
        }
       // if (!explosionJustTriggered) {
            randomRangeShootingCoroutine ??= StartCoroutine(RandomRangeShooting());

       // }
        triggerShieldCoroutine ??= StartCoroutine(TriggerShield());
        bigRangeAttackCooldownCoroutine ??= StartCoroutine(BigRangeAttackCooldown());
     
       
        UpdateTarget();
        if (isMoving) {
            Movement();
        }
    }

    private IEnumerator BigRangeAttack() {

        performingBigRangeAttack = true;
        
        Vector2 startPosition = transform.position;
        float journeyLength = Vector2.Distance(startPosition, bigrangeAttackPosition);
        float startTime = Time.time;

        while (Vector2.Distance(transform.position, bigrangeAttackPosition) > 0.1f) {
            // Calculate the fraction of the journey completed
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            // Move the transform position to the interpolated position
            transform.position = Vector2.Lerp(startPosition, bigrangeAttackPosition, fractionOfJourney);

            yield return null; // Wait until the next frame
        }

        // Ensure the position is exactly set at the target
        transform.position = bigrangeAttackPosition;
        yield return new WaitForSeconds(0.5f);
        bossBigProjectile = Instantiate(bigPlasmaProjectilePrefab, new Vector2(transform.position.x,transform.position.y - 0.5f), Quaternion.identity).GetComponent<BossBigProjectile>();
        Vector3 directionPlayer = (Player.Instance.transform.position - transform.position).normalized;
        bossBigProjectile.SetDirection(directionPlayer);
        yield return new WaitUntil(() => bossBigProjectile == null || bossBigProjectile.gameObject == null);
        performingBigRangeAttack = false;
        bigRangeAttackCoroutine = null;
        bigRangeAttackCooldownCoroutine = null;
        yield return new WaitForSeconds(0.5f);
        isMoving = true;
    }




    private IEnumerator BigRangeAttackCooldown() {
        canBigRangeAttack = false;
        yield return new WaitForSeconds(bigRangeAttackColldown);
        canBigRangeAttack = true;

    }   

    private IEnumerator RandomRangeShooting() {
        while (!performingMeleeAttack && !gettingPushed && !explosionJustTriggered) {
            //int projectileCount = smallRangeAttackProjectileNumber; // Number of projectiles to shoot at once
            //for (int i = 0; i < projectileCount; i++) {
            //    BossSmallProjectile bossSmallProjectile = Instantiate(smallProjectilePrefab, transform.position, Quaternion.identity).GetComponent<BossSmallProjectile>();


            //    Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            //    bossSmallProjectile.SetDirection(randomDirection);
            //}
            RandomRangeAttack(smallRangeAttackProjectileNumber,transform.position);

            yield return new WaitForSeconds(smallRangeAttackInterval);
        }

        randomRangeShootingCoroutine = null;
    }

    private void RandomRangeAttack(int projectileCount,Vector2 position) {
        for (int i = 0; i < projectileCount; i++) {
            BossSmallProjectile bossSmallProjectile = Instantiate(smallProjectilePrefab, position, Quaternion.identity).GetComponent<BossSmallProjectile>();


            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            bossSmallProjectile.SetDirection(randomDirection);

        }
    }


    private IEnumerator TriggerShield() {

        if (shieldOnCooldonwn) {
            yield return new WaitUntil(() => shieldOnCooldonwn == false);

        }
        int random = UnityEngine.Random.Range(0, 2);
        if (random == 1) {
           
            shieldCurrentHp = shield1MaxHp;
            shieldSlider.gameObject.SetActive(true);
            shieldSlider.maxValue = shield1MaxHp;
            shieldSlider.value = shield1MaxHp;
            shield1Active = true;
            shield2Active = false;
            shield1.SetActive(true);
            shield2.SetActive(false);
            shieldCollider.enabled = true;
        }
        else {
       
            shieldCurrentHp = shield2MaxHp;
            shieldSlider.gameObject.SetActive(true);
            shieldSlider.maxValue = shield2MaxHp;
            shieldSlider.value = shield2MaxHp;
            shield1Active = false;
            shield2Active = true;
            shield1.SetActive(false);
            shield2.SetActive(true);
            shieldSlider.gameObject.SetActive(true);
            shieldCollider.enabled = true;
        }
        yield return new WaitForSeconds(shieldResetCooldown);
        triggerShieldCoroutine = null;
    }
    private IEnumerator TriggerShieldCooldown() {
        shieldOnCooldonwn = true;
        yield return new WaitForSeconds(shieldResetCooldown +1);
        shieldOnCooldonwn = false;
        triggerShieldCooldownCoroutine = null;
    }
    protected override void Movement() {
        // if (phase1Active) {
        if(moveToEnemyPathCoroutine != null) {
            return;
        }
        int random = UnityEngine.Random.Range(0, 2);
        if (random == 0 && movement1Count < 2 || movement2Count > 2) {
            movement1Count++;
            if(movement2Count >= 2) {
                movement2Count = 0;
            }
            //moveToEnemyPathCoroutine ??= StartCoroutine(MoveInCircle(new Vector3(0,2), rotationSpeed, rotationDuration,2));
            moveToEnemyPathCoroutine ??= StartCoroutine(MoveInCircle(rotationSpeed, rotationDuration,2));
        }
        else {
            movement2Count++;
            if (movement1Count >= 2) {
                movement1Count = 0;
            }
            GetEnemyPath();
          
            moveToEnemyPathCoroutine ??= StartCoroutine(MoveToEnemyPath());


        }


        //}
        //else if(phase2Active) {
        //    if (moveToEnemyPathCoroutine == null) {
        //        moveToEnemyPathCoroutine = StartCoroutine(MoveToEnemyPath());

        //    }
        //}
    }
    private IEnumerator MoveInCircle( float speed, float duration, float radius) {

        float elapsedTime = 0f;
        float angle = 0f;
        rotateAroundPoint.transform.localPosition = new Vector3(0, 3.25f, 0f);

        Tween tween =  PointLeftRightMovement();

        while (elapsedTime < duration) {
            // Increment the angle based on rotation speed and elapsed time
            angle += speed * Time.deltaTime;

            // Convert angle to radians
            float angleRad = angle * Mathf.Deg2Rad;

            // Calculate the new position using the angle and radius
            float x = rotateAroundPoint.localPosition.x + radius * Mathf.Cos(angleRad);
            float y = rotateAroundPoint.localPosition.y + radius * Mathf.Sin(angleRad);

            // Update the enemy's position
            transform.position = new Vector3(x, y, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the rotation stops after the duration
        moveToEnemyPathCoroutine = null;
        tween.Kill();
       
    }

    private Tween PointLeftRightMovement() {
        int random = UnityEngine.Random.Range(0, 2);
        Vector3 targetPosition = new Vector3(random == 0 ? 5:-5, 3.25f, 0f);
        return  rotateAroundPoint.DOLocalMove(targetPosition, rotationDuration/2).SetEase(Ease.Linear).OnComplete(() => {
            targetPosition = new Vector3(random == 0 ? -5 : 5, 3.25f, 0f);
            rotateAroundPoint.DOLocalMove(targetPosition, rotationDuration/2).SetEase(Ease.Linear).OnComplete(() => {
               
            });
        });
    }


    private void GetEnemyPath() {
        // Determine the active phase
        List<string> activePatterns = phase1Active ? phase1MovementPatterns : phase2MovementPatterns;
        List<Vector2> activeLocations = phase1Active ? phase1MovementLocations : phase2MovementLocations;

        // Pick a random pattern from the active pattern list
        string randomPattern = activePatterns[UnityEngine.Random.Range(0, activePatterns.Count)];

        // List to store the positions based on the pattern
        currentEnemyPath = new List<Vector2>();

        // Parse the pattern and get the positions
        foreach (char c in randomPattern) {
            int index = int.Parse(c.ToString());
            if (index >= 0 && index < activeLocations.Count) {
                currentEnemyPath.Add(activeLocations[index]);
            }
        }

       
    }

    private IEnumerator MoveToEnemyPath() {
        foreach (Vector2 targetPosition in currentEnemyPath) {
            Vector2 startPosition = transform.position;
            float journeyLength = Vector2.Distance(startPosition, targetPosition);
            float startTime = Time.time;

            while ((Vector2)transform.position != targetPosition && isMoving) {
                // Calculate the fraction of the journey completed
                float distCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distCovered / journeyLength;

                // Move the transform position to the interpolated position
                transform.position = Vector2.Lerp(startPosition, targetPosition, fractionOfJourney);

                yield return null; // Wait until the next frame
            }

            // Ensure the position is exactly set at the target
            transform.position = targetPosition;

            // Wait for a specified time before moving to the next position
            yield return new WaitForSeconds(0.2f);
        }
            StartCoroutine(MovementCooldown());
        if (canBigRangeAttack) {
            bigRangeAttackCoroutine ??= StartCoroutine(BigRangeAttack());
        }
        else {
            StartCoroutine(MeleeAttack());

        }
        moveToEnemyPathCoroutine = null;
    }


    private IEnumerator MovementCooldown() {       
        isMoving = false;
        yield return new WaitForSeconds(2f);
        if(!performingMeleeAttack && !performingBigRangeAttack) {
            isMoving = true;
        }
    }
    private IEnumerator MeleeAttack() {
        performingMeleeAttack = true;
        Vector2 startPosition = transform.position;
        float journeyLength = Vector2.Distance(startPosition, hoverAbovePlayerPosition);
        float startTime = Time.time;

        while ((Vector2)transform.position != hoverAbovePlayerPosition) {
            // Calculate the fraction of the journey completed
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            // Move the transform position to the interpolated position
            transform.position = Vector2.Lerp(startPosition, hoverAbovePlayerPosition, fractionOfJourney);

            yield return null; // Wait until the next frame
        }

        // Ensure the position is exactly set at the target
        transform.position = hoverAbovePlayerPosition;
        yield return new WaitForSeconds(0.5f);
        checkForCoinSukerCoroutine ??= StartCoroutine(CheckForCoinSuker());
        if(phase2Active) {

            while (performingMeleeAttack) {
                Player.Instance.TakeDamage(meleeAttackDamage);
                Debug.Log("mele attack triggred");
                yield return new WaitForSeconds(meleeAttackDmgInterval);
            }
        }
        else {
            while (performingMeleeAttack) {
               
                yield return new WaitForSeconds(meleeAttackDmgInterval);
            }
        }
     

    }

    private IEnumerator CheckForCoinSuker() {
        float timer = 0f;
        while (performingMeleeAttack) {
            if (CoinSucker.Instance.isSuckerActive) {
                performingMeleeAttack = false;
                checkForCoinSukerCoroutine = null;
                getPushedAwayCoroutine ??= StartCoroutine(GetPushedAway());
                if (timer <= (phase1Active? 5 : 3)) {
                    DestroyShield();
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
        checkForCoinSukerCoroutine = null;
    }



    protected override void OnTriggerEnter2D(Collider2D collision) {
        //if ((collision.CompareTag("Player") || collision.CompareTag("WarriorGotchi")) && !isMoving) {
        //    potentialTargets.Add(collision.transform);
        //    UpdateTarget();
        //}
        if (collision.CompareTag("PlayerProjectile")) {
            if (performingMeleeAttack && ProjectileThrower.Instance.isThrowerActive) {
                Player.Instance.TakeDamage(reflectedDmgToPlayer);
                return;
            }
            if (shield2Active) {
                RandomRangeAttack(1,collision.transform.position);
            }
        }
        //else if (collision.CompareTag("EnemyProjectile")) {
        //  BossSmallProjectile bossSmallProjectile =  collision.GetComponent<BossSmallProjectile>();
        //    if(bossSmallProjectile != null) {
        //        if (bossSmallProjectile.transformed) {

        //        }
        //    }
        //}

    }

    protected override void OnTriggerExit2D(Collider2D collision) {
        //if (potentialTargets.Contains(collision.transform)) {
        //    potentialTargets.Remove(collision.transform);
        //    UpdateTarget();
        //}
    }

    protected override void OnCollisionStay2D(Collision2D collision) {
     

    }


    protected override IEnumerator DealDamageRepeatedly(Collider2D targetCollider) {
        while (!isFrozen && !isDead && targetCollider != null) {
            targetCollider.GetComponent<Player>()?.TakeDamage(damageToPlayer);
            targetCollider.GetComponent<WarriorGotchi>()?.TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
        }
    }



    public override void TakeDamage(int damage, bool isExplosionDmg) {
      
        if (isDead) return;

        if(performingMeleeAttack) {
            return;
        }


        if(shield1Active || shield2Active) {
            TakeShieldDamage(damage);
           
            return;
        }
        
        if (!isFrozen || SupportThrower.Instance.canDamageFrozenEnemies) {
            if (takesObjectiveDamage) {
                currentHealth -= objectiveOnHitDamageTaken;

                DisplayDamage(objectiveOnHitDamageTaken, transform.position);
            }
            else {
                currentHealth -= damage;
                DisplayDamage(damage, transform.position);
            }
           

        }
        // Play damage sound effect
        if (damageSound != null) {
            damageSound.Play();
        }
        if(currentHealth < baseHealth / 2) {
            phase2Active = true;
            phase1Active = false;

        }

        if (currentHealth <= 0) {
            Die();
        }
    }


    protected override void Die() {
        if (isDead) return; // Prevent multiple deaths
        isDead = true; // Mark as dead

        // Disable all colliders on the enemy
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders) {
            collider.enabled = false;
        }

        // Play death sound effect
        if (deathSound != null) {
            deathSound.Play();
        }

        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null) {
            resourceManager.AddScore(100);
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        BattleManager.Instance.EndRound();
        StartCoroutine(FadeOut(1f)); // Fade out over 1 second
    }


    public override void SetHealth(int level) {
        int healthIncrease = 20; // Default for Easy

        switch (GameSettings.Instance.currentDifficulty) {
            case GameSettings.Difficulty.Medium:
                healthIncrease = 30;
                break;
            case GameSettings.Difficulty.Hard:
                healthIncrease = 50;
                break;
        }

        baseHealth += 30 + (level - 1) * healthIncrease;
        currentHealth = baseHealth;
    }

    public override void Freeze(bool solidFreeze) {

        //if (freezeCoroutine != null) {
        //    StopCoroutine(freezeCoroutine);
        //    freezeCoroutine = null;
        //}

        //spriteRenderer.color = Color.blue;
        //isFrozen = true;
        //if (solidFreeze) {
        //    ToggleObstacleCollider(true);
        //    freezeCoroutine = StartCoroutine(UnfreezeAfterDuration(SupportProjectile.freezeDuration, true));
        //}
        //else {
        //    freezeCoroutine = StartCoroutine(UnfreezeAfterDuration(SupportProjectile.freezeDuration, false));
        //}

    }


    public void TakeShieldDamage(int damage) {
    
        if (takesObjectiveDamage) {
            shieldCurrentHp -= objectiveOnHitDamageTaken;
            shieldSlider.value -= objectiveOnHitDamageTaken;
            DisplayDamage(objectiveOnHitDamageTaken, transform.position);
        }
        else {
            shieldCurrentHp -= damage;
            shieldSlider.value -= damage;
            DisplayDamage(damage, transform.position);
        }

        if(shieldCurrentHp <= 0) {
            DestroyShield();
        }


    }

    public void DestroyShield() {
        if (shield1Active || shield2Active) {
            shieldCurrentHp = 0;
            shieldSlider.gameObject.SetActive(false);
            shield1Active = false;
            shield2Active = false;
            shield1.SetActive(false);
            shield2.SetActive(false);
            shieldCollider.enabled = false;
            triggerShieldCooldownCoroutine ??= StartCoroutine(TriggerShieldCooldown());
          
        }
    }

    private IEnumerator GetPushedAway() {
        gettingPushed=true;
        Vector2 startPosition = transform.position;
        Vector2 destination = new Vector2(hoverAbovePlayerPosition.x, hoverAbovePlayerPosition.y + 4f);
        float journeyLength = Vector2.Distance(startPosition, destination);
        float startTime = Time.time;
        while (Vector2.Distance(transform.position,destination)>0.2f) {
            // Calculate the fraction of the journey completed
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            // Move the transform position to the interpolated position
            transform.position = Vector2.Lerp(startPosition, destination, fractionOfJourney);

            yield return null; // Wait until the next frame
        }
        gettingPushed=false;
        isMoving = true;
        getPushedAwayCoroutine = null;
    }

    public void TriggerExplostion() {

        OnExplosionTriggerd?.Invoke(this, EventArgs.Empty);
        bossBigProjectile = null;
        //explosionJustTriggeredCortuine ??= StartCoroutine(ExplosionEffect());
    }

    private IEnumerator ExplosionEffect() {
        explosionJustTriggered = true;
        yield return new WaitForSeconds(10f);
        explosionJustTriggered = false;
        explosionJustTriggeredCortuine = null;
    }
}
