using System.Collections;
using UnityEngine;


public class Projectile : MonoBehaviour {
    public float speed = 10f; // Speed of the projectile
    public static int damageAmount = 0; // Amount of damage dealt to enemies
    public static int projectileBonusDamage = 0;
    public static int weaponBonusDamage = 20; // Amount of damage dealt to enemies
    public float destroyDelay = 10f; // Delay before destroying the projectile
    [SerializeField] protected bool destoryedOnBorderInpact;
    [SerializeField] protected bool destoryedOnEnemyInpact;

    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform canvasTransform; // Reference to the transform of the Canvas object
    [SerializeField] protected SoundData shootProjectileSoundData;
  

    protected Vector3 direction; // Direction in which the projectile will move
    public SoundManager soundManager => SoundManager.Instance;
    protected virtual void Awake() {
        if (UIManager.Instance.battleCanvasTransform == null) {
            Debug.LogError("BattleCanvas Transform is not set in the UIManager.");
            return;
        }
        canvasTransform = UIManager.Instance.battleCanvasTransform;


    }
    protected virtual void Start() {
        StartCoroutine(DestroyAfterDelay());
        InitSoundSettings();
        if (shootProjectileSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            soundManager.CreateSound().WithSoundData(shootProjectileSoundData).WithPosition(transform.position).Play();

        }
        
    }
    protected virtual void InitSoundSettings() {
        shootProjectileSoundData.loop = false;
        shootProjectileSoundData.frequentSound = true;
     


        SetMusicClip();
    }
    protected virtual void SetMusicClip() {
        var projectilesCategory = soundManager.audioGalleryEntries.ProjectilesCategory;
        foreach (var field in projectilesCategory.GetAudioClipFields()) {
            // Matching the name convention for OnHit sounds
            if (field.Name == this.GetType().Name + "OnShoot") {
                // Get the value from the scriptable object field
                AudioClip clip = (AudioClip)field.GetValue(projectilesCategory);
                // Assign it to your local variable
                shootProjectileSoundData.clip = clip;
            }
        }
     
    }
    protected virtual void Update() {
        // Move the projectile in its direction
        transform.position += direction * speed * Time.deltaTime;
    }

    public static void UpdateDamageAmount() {
        damageAmount = projectileBonusDamage + weaponBonusDamage;

    }

    // Method to set the direction of the projectile
    public virtual void SetDirection(Vector3 dir) {
        direction = dir;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        // Check if the collider is an enemy
        if (other.CompareTag("Enemy")) {
            // Get the Enemy component from the collided object
            Enemy enemy = other.GetComponent<Enemy>();

            // If the enemy component exists, apply damage
            if (enemy != null) {
                

                enemy.TakeDamage(damageAmount, false);
                // DisplayDamage(damageAmount, transform.position);
                if (destoryedOnEnemyInpact) {
                    Destroy(gameObject);
                }
            }
        }
        // Additional condition for CoinStealer
        else if (other.CompareTag("CoinStealer")) {
            // Get the CoinStealer component from the collided object
            CoinStealer coinStealer = other.GetComponent<CoinStealer>();

            // If the CoinStealer component exists, apply damage
            if (coinStealer != null) {
                coinStealer.TakeDamage(damageAmount);
                // DisplayDamage(damageAmount, transform.position);
                if (destoryedOnEnemyInpact) {
                    Destroy(gameObject);
                }
            }
        }
        //else if (other.CompareTag("PassiveEnemy")) {
           
        //}
        if (other.CompareTag("Border") && destoryedOnBorderInpact) {
            Destroy(gameObject);
        }

    }


    //void DisplayDamage(int damage, Vector3 position) {
    //    GameObject damageTextObject = Instantiate(damageTextPrefab, position, Quaternion.identity, canvasTransform);
    //    Text textComponent = damageTextObject.GetComponent<Text>();
    //    textComponent.text = damage.ToString("F0");
    //    // Ensure the text is visible above everything else
    //    damageTextObject.transform.localPosition = new Vector3(damageTextObject.transform.localPosition.x, damageTextObject.transform.localPosition.y, 0);
    //}

    // Coroutine to destroy the projectile after a delay
    protected virtual IEnumerator DestroyAfterDelay() {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}