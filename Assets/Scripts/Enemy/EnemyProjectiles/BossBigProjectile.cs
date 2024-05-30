using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBigProjectile : EnemyProjectile {
    [SerializeField] protected bool destroysPlayerBulletsAlso;
    [SerializeField] protected int maxHealth = 7;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int maxHitBackCount = 5;
    [SerializeField] protected float speedAddedPerBounce = 0.2f;
    public int hitBackCountPlayer;
    public int hitBackCountBoss;
    public SpriteRenderer spriteRenderer;
    public Color baseColor;
    public bool transformed;
    private Coroutine ricochetCoroutine;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;

    }
    protected override void Start() {
        base.Start();
        currentHealth = maxHealth;
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
                if (GoldenSnitchBoss.Instance == null) {
                    Destroy(gameObject);
                    return;
                }
                if (destroysPlayerBulletsAlso) {
                    Destroy(other.gameObject);
                }
                currentHealth --;
                
                if (currentHealth <= 0) {

                    transformed = true;
                    spriteRenderer.color = Color.red;

                    Vector3 directionBack = (GoldenSnitchBoss.Instance.transform.position - transform.position).normalized;
                    speed += speedAddedPerBounce;
                    SetDirection(directionBack);
                    hitBackCountPlayer++;
                    currentHealth = maxHealth;
                }
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                if (hitBackCountBoss > 7) {
                    GoldenSnitchBoss goldenSnitchBoss = other.GetComponent<GoldenSnitchBoss>();
                    int random = Random.Range(0, 3);
                    if(random == 0) {
                        ricochetCoroutine ??= StartCoroutine(Ricochet(goldenSnitchBoss));
                        return;
                    }
                    if(goldenSnitchBoss.shield1Active || goldenSnitchBoss.shield2Active) {
                        goldenSnitchBoss.DestroyShield();
                        Destroy(gameObject);
                    }
                    else {
                        goldenSnitchBoss.TakeDamage(damageAmount, false);
                        goldenSnitchBoss.TakeDamage(damageAmount, false);
                        goldenSnitchBoss.TakeDamage(damageAmount, false);
                        goldenSnitchBoss.TakeDamage(damageAmount, false);
                        goldenSnitchBoss.TakeDamage(damageAmount, false);
                        Destroy(gameObject);

                    }

                   
                }
                else {
                    transformed = false;
                    spriteRenderer.color = baseColor;
                    speed += speedAddedPerBounce;
                    Vector3 directionPlayer = (Player.Instance.transform.position - transform.position).normalized;
                    SetDirection(directionPlayer);
                    hitBackCountBoss++;
                }
            }


        }
    }


    private IEnumerator Ricochet(GoldenSnitchBoss goldenSnitchBoss) {
        Vector2 startPosition = transform.position;
        Vector2 destination = new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 4f);
        float journeyLength = Vector2.Distance(startPosition, destination);
        float startTime = Time.time;
        while ((Vector2)transform.position != destination) {
            // Calculate the fraction of the journey completed
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;

            // Move the transform position to the interpolated position
            transform.position = Vector2.Lerp(startPosition, destination, fractionOfJourney);

            yield return null; // Wait until the next frame
        }
        goldenSnitchBoss.TriggerExplostion();
        ricochetCoroutine = null;
        //after animation
        Destroy(gameObject);
    }
}
