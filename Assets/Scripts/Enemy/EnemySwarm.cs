using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : Enemy
{
    private float destructionTimer; // Time in seconds before destruction
    private bool beeingSuckedIn;
    [SerializeField]private float destructionTimerMax = 3f; 
    protected override void Start() {
        base.Start();
        destructionTimer = destructionTimerMax;
    }

    protected override void Update() {
        base.Update();

        if (CoinSucker.Instance.isSuckerActive ) {
            beeingSuckedIn = true;
            transform.position = Vector2.MoveTowards(transform.position, target.position + new Vector3(0, 1f), CoinSucker.Instance.SuckPower * Time.deltaTime);
            // Reduce the destruction timer
            destructionTimer -= Time.deltaTime;

            // Check if the destruction timer has reached zero
            if (destructionTimer <= 0f) {
                // Destroy the object
                Destroy(gameObject);
            }
        }
        else {
            beeingSuckedIn = false;
            // Reset the destruction timer if the condition is not met
            destructionTimer = destructionTimerMax; // Reset to original value
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
}
