using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBigProjectile : EnemyProjectile {
    [SerializeField] protected bool destroysPlayerBulletsAlso;
    [SerializeField] protected int maxHealth = 7;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int maxHitBackCount = 5;
    [SerializeField] protected float speedAddedPerBounce = 0.2f;
    [SerializeField] protected SpriteRenderer visual1;
    [SerializeField] protected SpriteRenderer visual2;
    [SerializeField] private Collider2D suckInCollider;
    public int hitBackCountPlayer;
    public int hitBackCountBoss;
    public Color baseColor;
    public bool transformed;
    public bool ricochet;
    private Coroutine ricochetCoroutine;
    private Coroutine explosionPulseCoroutine;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        baseColor = spriteRenderer.color;

    }
    protected override void Start() {
        base.Start();
        currentHealth = maxHealth;
        hitBackCountBoss = 0;
    }

    protected override void Update() {
        if (ricochet) {
            return;
        }
        base.Update();
    }


    protected override void OnTriggerEnter2D(Collider2D other) {
      
        if (!transformed) {
            if (other.CompareTag("Player")) {
                Player player = other.GetComponent<Player>();
                if (player != null) {
                    player.TakeDamage(damageAmount);
                }
                Debug.Log("Hit player");
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
            //else {
            //    Debug.Log("buged transformed:"+transformed);
            //}
        }
        else {
            if (other.CompareTag("Enemy")) {
                if (hitBackCountBoss > 7) {
                    GoldenSnitchBoss goldenSnitchBoss = other.GetComponent<GoldenSnitchBoss>();
                    int random = Random.Range(0, 3);
                    if(random == 0) {
                        ricochetCoroutine ??= StartCoroutine(Ricochet(goldenSnitchBoss));
                        Debug.Log("ricochet");
                    }
                    else if(goldenSnitchBoss.shield1Active || goldenSnitchBoss.shield2Active) {
                        goldenSnitchBoss.DestroyShield();
                        Destroy(gameObject);
                        Debug.Log("Hit boss shield");
                    }
                    else {
                        Debug.Log("Hit boss hp");
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

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("EnemyProjectile") && ricochet) {
            if (Vector2.Distance(new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 3f), other.transform.position) < 0.3f) {
                Debug.Log(Vector2.Distance(new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 3f), other.transform.position));
                Destroy(other.gameObject);
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if( collision != null ) {
           if(collision.gameObject.CompareTag("EnemyProjectile") && ricochet) {

                //if (Vector2.Distance(new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 3f), collision.transform.position) < 0.3f) {
                //    Debug.Log(Vector2.Distance(new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 3f), collision.transform.position));
                //    Destroy(collision.gameObject);
                //}
                //else {
                    Vector2  direction = (transform.position - collision.transform.position).normalized;
                    EnemyProjectile enemyProjectile = collision.gameObject.GetComponent<EnemyProjectile>();
                    enemyProjectile.SetDirection(direction);
                    enemyProjectile.speed = 1f;

               // }


                
           }


        }
    }


    private IEnumerator Ricochet(GoldenSnitchBoss goldenSnitchBoss) {
        ricochet = true;
        spriteRenderer.color = Color.white;
        suckInCollider.enabled = true;
        Vector2 startPosition = transform.position;
        Vector2 destination = new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y + 3f);
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
       // ricochetCoroutine = null;
        visual1.sprite = null;
        visual2.sprite = null;

        animator.Play("bossExplosionAnimation");
        explosionPulseCoroutine ??= StartCoroutine(ExplosionPulse());


    }

    private IEnumerator ExplosionPulse() {
        yield return new WaitForSeconds(10);
        DestroyGameobjectAfterExplosion();
      //  animator.Play("bossPulseAnimation");

    }

    public void DestroyGameobjectAfterExplosion() {
        ricochet = false;
        Destroy(gameObject);
    }
}
