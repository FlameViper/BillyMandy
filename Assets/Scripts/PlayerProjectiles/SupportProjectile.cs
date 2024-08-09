using System.Collections;
using UnityEngine;
using System;

public class SupportProjectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public static float freezeDuration = 2f; // Static freezeDuration to allow global access and modification
    public float destroyDelay = 10f; // Delay before destroying the projectile

    private Vector3 direction; // Direction in which the projectile will move
    [SerializeField] protected SoundData shootProjectileSoundData;
    public SoundManager soundManager => SoundManager.Instance;
    protected virtual void Start()
    {
        StartCoroutine(DestroyAfterDelay());
        InitSoundSettings();
        if (shootProjectileSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            soundManager.CreateSound().WithSoundData(shootProjectileSoundData).WithPosition(transform.position).Play();

        }

    }
    protected virtual void InitSoundSettings() {
        shootProjectileSoundData.loop = false;
        shootProjectileSoundData.frequentSound = false;


        Type enemyType = this.GetType();
        var projectileCategory = soundManager.audioGalleryEntries.ProjectilesCategory;

        // Set the OnHit sound data
        SoundManager.SetAudioClipForType(enemyType, projectileCategory, shootProjectileSoundData, "OnShoot");


    }

    protected virtual void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                FreezeEnemy(enemy);
               
            }

      
        }
    }

    private void FreezeEnemy(Enemy enemy) {
        if (enemy != null) {
            enemy.Freeze(false); // Start freezing

        }
    }


    // Coroutine to destroy the projectile after a delay
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
