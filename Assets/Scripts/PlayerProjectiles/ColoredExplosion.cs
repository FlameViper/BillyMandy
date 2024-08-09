using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredExplosion : MonoBehaviour {
    Animator animator;
    [SerializeField] private string color;
    [SerializeField] private float destroyDelay = 0.5f;
    [SerializeField] SoundData explosionSound;
    public SoundManager soundManager => SoundManager.Instance;
    private int numberOfColoredEnemies;
    bool exploded;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        PlayColoredExplosion();
        StartCoroutine(DestroyAfterDelay());
        InitSoundSettings();
        if (explosionSound.clip != null && !GameSettings.Instance.SFXOFF) {
            Debug.Log("triggred colored explosion");
            soundManager.CreateSound().WithSoundData(explosionSound).WithPosition(transform.position).Play();

        }
    }
    protected virtual void InitSoundSettings() {
        explosionSound.loop = false;
        explosionSound.frequentSound = false;
        SetMusicClip();
    }
    public void SetMusicClip() {
        var projectilesCategory = soundManager.audioGalleryEntries.ProjectilesCategory;
        foreach (var field in projectilesCategory.GetAudioClipFields()) {
            // Matching the name convention for OnHit sounds
      
            if (field.Name == GetType().Name + "OnShoot") {
              
                // Get the value from the scriptable object field
                AudioClip clip = (AudioClip)field.GetValue(projectilesCategory);
                // Assign it to your local variable
                explosionSound.clip = clip;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {
   
        if (collision == null) {
            return;
        }
        if (collision.CompareTag("Enemy")  /*&& exploded*/) {
            //if its explosion dmg it dosent triggr others explosions
            collision.gameObject.GetComponent<Enemy>().TakeDamage(200, true);


        }
    }



    private void PlayColoredExplosion() {
        switch (color) {
            case "Red":
                animator.Play("coloredExplosionRed");
                break;
            case "Green":
                animator.Play("coloredExplosionGreen");
                break;
            case "Yellow":
                animator.Play("coloredExplosionYellow");
                break;
            case "Purple":
                animator.Play("coloredExplosionPurple");
                break;
            default:
                break;
        }
    }

    public void DestroyAfterAniamtion() {
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDelay() {
        yield return new WaitForSeconds(destroyDelay);
        if (!exploded) {
            Destroy(gameObject);

        }
    }
}
