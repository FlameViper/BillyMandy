using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : Enemy
{
    const string FLASH_AMOUNT = "_FlashAmount";
    public float destructionTimer; // Time in seconds before destruction
    public float modifiedDestructionTimer; // Time in seconds before destruction
    private bool beeingSuckedIn;
    private float hurtAnimationTimer;
    private Coroutine hurtAnimationCorutine;
    [SerializeField]private float destructionTimerMax = 3f;
    [SerializeField] private float intervalBetweenHurtAnimations = 1f;
    [SerializeField]private float nightTimeModifier = 0.5f;
    [SerializeField]private float perLvlModifier = 0.1f;
    [SerializeField]private float dificultyModifierEasy = 0.3f;
    [SerializeField]private float dificultyModifierMedium = 0.4f;
    [SerializeField]private float dificultyModifierHard = 0.5f;
    [SerializeField] private float HurtAnimationDelay = 0.2f;
    [SerializeField] private bool isNightTime; //for testing until i code the nightTime feature
    protected override void Start() {
        base.Start();
       
        destructionTimer = GetDestructionTimer();
        modifiedDestructionTimer = GetDestructionTimer();
    }

    protected override void Update() {
        base.Update();

        if (CoinSucker.Instance.isSuckerActive ) {
            beeingSuckedIn = true;
            transform.position = Vector2.MoveTowards(transform.position, target.position + new Vector3(0, 1f), CoinSucker.Instance.SuckPower * Time.deltaTime);
            // Reduce the destruction timer
            destructionTimer -= Time.deltaTime;
            // Call the coroutine every second
            hurtAnimationTimer -= Time.deltaTime;
            if (hurtAnimationTimer <= 0f) {
                hurtAnimationCorutine ??= StartCoroutine(HandleEnemyHurtAnimation());
                hurtAnimationTimer = intervalBetweenHurtAnimations; // Reset the timer
            }
            // Check if the destruction timer has reached zero
            if (destructionTimer <= 0f) {
                // Destroy the object
                Destroy(gameObject);
            }
        }
        else {
            beeingSuckedIn = false;
            // Reset the destruction timer if the condition is not met
            destructionTimer = modifiedDestructionTimer; // Reset to original value
        }
    }


    protected override void OnCollisionStay2D(Collision2D collision) {


    }
    public override void TakeDamage(int damage, bool isExplosionDmg) {
       
    }

    protected override IEnumerator DealDamageRepeatedly(Collider2D targetCollider) {
        while (!isFrozen && !isDead && !beeingSuckedIn && targetCollider != null) {
            targetCollider.GetComponent<Player>()?.TakeDamage(damageToPlayer);
            targetCollider.GetComponent<WarriorGotchi>()?.TakeDamage(damageToPlayer);
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    private float GetDestructionTimer() {
        float modifiedTimer = destructionTimerMax;

        switch (GameSettings.Instance.currentDifficulty) {
            case GameSettings.Difficulty.Easy:
                modifiedTimer += perLvlModifier * BattleManager.Instance.level;
                modifiedTimer += isNightTime ? nightTimeModifier : 0f;
                modifiedTimer += dificultyModifierEasy;
                break;

            case GameSettings.Difficulty.Medium:
                modifiedTimer += perLvlModifier * BattleManager.Instance.level;
                modifiedTimer += isNightTime ? nightTimeModifier : 0f;
                modifiedTimer += dificultyModifierMedium;
                break;

            case GameSettings.Difficulty.Hard:
                modifiedTimer += perLvlModifier * BattleManager.Instance.level;
                modifiedTimer += isNightTime ? nightTimeModifier : 0f;
                modifiedTimer += dificultyModifierHard;
                break;

            default:
                Debug.LogWarning("Unknown difficulty setting!");
                break;
        }
        
        return modifiedTimer;
    }
    public override void Freeze(bool solidFreeze) {

        return;
    }

    public IEnumerator HandleEnemyHurtAnimation() {

        spriteRenderer.material.SetFloat(FLASH_AMOUNT, 1);
        if (enemyOnHitSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            //Debug.Log("played");
            soundManager.CreateSound().WithSoundData(enemyOnHitSoundData).WithPosition(transform.position).Play();

        }
        yield return new WaitForSeconds(HurtAnimationDelay);

        spriteRenderer.material.SetFloat(FLASH_AMOUNT, 0);
        hurtAnimationCorutine = null;
    }

}
