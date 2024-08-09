using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] protected int baseHealth = 50;
    public int currentHealth;
    public int damageToPlayer = 10;
    public float stopDistance = 1f;
    public float attackSpeed = 1f;
    public GameObject coinPrefab;
    [SerializeField] private Collider2D obstacleCollider;
    public AudioSource damageSound;
    public AudioSource deathSound;

    protected Transform target;
    protected Transform player;
    protected Coroutine attackRoutine = null;
    protected SpriteRenderer spriteRenderer;
    protected Color baseColor;
    protected Rigidbody2D rb;
    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform canvasTransform; // Reference to the transform of the Canvas object


    protected bool isDead = false;
    protected bool isTouchingFrozenEnemy = false;
    protected bool isFrozen = false;
    protected Coroutine freezeCoroutine;

    //sound
    [SerializeField] protected SoundData enemyOnHitSoundData;
    [SerializeField] protected SoundData enemyOnDeathSoundData;

    public SoundManager soundManager => SoundManager.Instance;

    protected List<Transform> potentialTargets = new List<Transform>();
    protected virtual void Awake() {
        if (UIManager.Instance.battleCanvasTransform == null) {
            Debug.LogError("BattleCanvas Transform is not set in the UIManager.");
            return;
        }
        canvasTransform = UIManager.Instance.battleCanvasTransform;
    }
    protected virtual void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();   
        baseColor = spriteRenderer.color;
        InitSoundSettings();

    }
    protected virtual void InitSoundSettings() {
        enemyOnHitSoundData.loop = false;
        enemyOnDeathSoundData.loop = false;
        enemyOnHitSoundData.frequentSound = false;
        enemyOnDeathSoundData.frequentSound = false;


        Type enemyType = this.GetType();
        var enemyOnDeathCategory = soundManager.audioGalleryEntries.EnemyOnDeathCategory;
        var enemyOnHitCategory = soundManager.audioGalleryEntries.EnemyOnHitCategory;

        // Set the OnHit sound data
        SoundManager.SetAudioClipForType(enemyType, enemyOnHitCategory, enemyOnHitSoundData, "OnHit");

        // Set the OnDeath sound data
        SoundManager.SetAudioClipForType(enemyType, enemyOnDeathCategory, enemyOnDeathSoundData, "OnDeath");
    }
    protected virtual void Update()
    {
        if (isTouchingFrozenEnemy && !isFrozen && !isDead) {
            // Calculate the direction to move towards the target
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > stopDistance) {
                transform.position = Vector2.MoveTowards(transform.position, target.position, 0.1f * Time.deltaTime);
            }

            return;
        }
        if (isDead || isFrozen) {
            
            return;
        }
            

        // Update target to the closest potential target
        UpdateTarget();

        Movement();
    }

    protected virtual void Movement() {

        if (target != null) {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > stopDistance) {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("WarriorGotchi"))
        {
           
            potentialTargets.Add(collision.transform);
            UpdateTarget();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (potentialTargets.Contains(collision.transform))
        {
         
            potentialTargets.Remove(collision.transform);
            UpdateTarget();
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag("Enemy")) {
            if (collision.collider.name == "Frozen") {
                isTouchingFrozenEnemy = true;
               
            }
            else {
                isTouchingFrozenEnemy = false;
            }


        }

    }

    protected virtual void UpdateTarget()
    {
        if (potentialTargets.Count == 0)
        {
            target = player;  // Default back to player if no gotchis are close
            return;
        }
       
        Transform closest = null;
        float minDistance = float.MaxValue;
        foreach (Transform t in potentialTargets)
        {
            if(t !=null) {
                float dist = Vector2.Distance(transform.position, t.position);
                if (dist < minDistance)
                {
                    closest = t;
                    minDistance = dist;
                }           
            }
            else {
                Debug.Log(t +"null");

            }
        }
   
        if (closest != null && closest != target)
        {
  
            target = closest;
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
            attackRoutine = StartCoroutine(DealDamageRepeatedly(target.GetComponent<Collider2D>()));
        }
    }

    protected virtual IEnumerator DealDamageRepeatedly(Collider2D targetCollider)
    {
        while (!isFrozen && !isDead && targetCollider != null)
        {
            targetCollider.GetComponent<Player>()?.TakeDamage(damageToPlayer);
            targetCollider.GetComponent<WarriorGotchi>()?.TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
        }
    }



    public virtual void TakeDamage(int damage, bool isExplosionDmg)
    {
        // Ignore damage if already dead
        if (isDead) return;
        if(!isFrozen || SupportThrower.Instance.canDamageFrozenEnemies) {
            currentHealth -= damage;
            DisplayDamage(damage, transform.position);

        }
        // Play damage sound effect

        if(enemyOnHitSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            //Debug.Log("played");
            soundManager.CreateSound().WithSoundData(enemyOnHitSoundData).WithPosition(transform.position).Play();

        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }


    protected virtual void Die()
    {
        if (isDead) return; // Prevent multiple deaths
        isDead = true; // Mark as dead

        // Disable all colliders on the enemy
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        // Play death sound effect
        if (enemyOnDeathSoundData.clip != null && !GameSettings.Instance.SFXOFF)
        {
            soundManager.CreateSound().WithSoundData(enemyOnDeathSoundData).WithPosition(transform.position).Play();
        }


        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddScore(100);
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);

        StartCoroutine(FadeOut(1f)); // Fade out over 1 second
    }


    protected IEnumerator FadeOut(float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }
        Destroy(gameObject); // Destroy the object after fading out
    }

    public virtual void SetHealth(int level)
    {
        int healthIncrease = 20; // Default for Easy

        switch (GameSettings.Instance.currentDifficulty)
        {
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

    public virtual void Freeze(bool solidFreeze) {

        if(freezeCoroutine != null) {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }

        spriteRenderer.color = Color.blue;
        isFrozen = true;
        if (solidFreeze) {
            ToggleObstacleCollider(true);
            freezeCoroutine = StartCoroutine(UnfreezeAfterDuration(SupportProjectile.freezeDuration, true));
        }
        else {
            freezeCoroutine = StartCoroutine(UnfreezeAfterDuration(SupportProjectile.freezeDuration, false));
        }
       
    }

    protected IEnumerator UnfreezeAfterDuration(float duration, bool solidFreeze) {

        yield return new WaitForSeconds(duration);
        spriteRenderer.color = baseColor;
        isFrozen = false;
        if (solidFreeze) {
            ToggleObstacleCollider(false);
        }
        freezeCoroutine = null;
    }


    protected void ToggleObstacleCollider(bool value) {
       
        obstacleCollider.name = value ? "Frozen" : "Normal";
    }

    protected void DisplayDamage(int damage, Vector3 position) {
        GameObject damageTextObject = Instantiate(damageTextPrefab, position, Quaternion.identity, canvasTransform);
        Text textComponent = damageTextObject.GetComponent<Text>();
        textComponent.text = damage.ToString("F0");
        // Ensure the text is visible above everything else
        damageTextObject.transform.localPosition = new Vector3(damageTextObject.transform.localPosition.x, damageTextObject.transform.localPosition.y, 0);
    }

   
}