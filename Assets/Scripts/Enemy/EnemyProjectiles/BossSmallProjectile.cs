using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSmallProjectile : EnemyProjectile {

    [SerializeField] protected bool destroysPlayerBulletsAlso;
    [SerializeField] protected int maxHealth = 10;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int maxHitBackCount=5;
    public int hitBackCountPlayer;
    public int hitBackCountBoss;
    public SpriteRenderer spriteRenderer;
    public Color baseColor;
    public bool transformed;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        
    }
    protected override void Start() {
        base.Start();
        currentHealth = maxHealth;
        GoldenSnitchBoss.Instance.OnExplosionTriggerd += Instance_OnExplosionTriggerd;
    }
    private void OnDestroy() {
        GoldenSnitchBoss.Instance.OnExplosionTriggerd -= Instance_OnExplosionTriggerd;
    }
    private void Instance_OnExplosionTriggerd(object sender, System.EventArgs e) {
     
        Destroy(gameObject);

        
    }

    protected override void Update() {
        base.Update();

    }


    protected override void OnTriggerEnter2D(Collider2D other) {
        if (!transformed) {
            if (other.CompareTag("Player")) {
                Player player = other.GetComponent<Player>();
                if (player != null) {
                    player.TakeDamage(damageAmount);
                }
                Destroy(gameObject);
            }
            else if (other.CompareTag("PlayerProjectile")) {
                if(GoldenSnitchBoss.Instance == null) {
                    Destroy(gameObject);
                    return;
                }
                if (destroysPlayerBulletsAlso) {
                    Destroy(other.gameObject);
                }
                currentHealth -= Projectile.damageAmount;
                if (currentHealth <= 0) {

                    if (GoldenSnitchBoss.Instance.phase1Active) {
                        Destroy(gameObject);

                    }
                    else {
                        if (hitBackCountPlayer > 5) {
                            Destroy(gameObject);
                        }
                        transformed = true;
                        spriteRenderer.color = Color.red;

                        Vector3 directionBack = (GoldenSnitchBoss.Instance.transform.position - transform.position).normalized;

                        SetDirection(directionBack);
                        hitBackCountPlayer++;
                    }
                }
            }
        }
        else{
            if (other.CompareTag("EnemyProjectile") ) {
                BossBigProjectile isBossProjectile;
                other.TryGetComponent(out isBossProjectile);
                if(isBossProjectile != null) {
                    Destroy(gameObject);
                    return;
                }
                Destroy(other.gameObject);
                if (hitBackCountBoss > 5) {
                    Destroy(gameObject);
                }
                spriteRenderer.color = baseColor;
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                SetDirection(randomDirection);
                hitBackCountBoss++;
            }
            else if (other.CompareTag("Enemy")) {
                other.GetComponent<Enemy>().TakeDamage(damageAmount,false);
 
                Destroy(gameObject);
            }


        }
    }
}
