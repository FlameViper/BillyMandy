using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalisticProjectile : Projectile
{
    // Start is called before the first frame update
    private Animator animator;
    private Vector3 inpactPosition;
    [SerializeField] private Collider2D inpactCollider;
    [SerializeField] SoundData impactProjectileSoundDatas;
    protected override void Awake() {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
    }
    protected override void Start()
    {
        inpactCollider.enabled = false;
        HandleProjectile();
        InitSoundSettings();
        if (shootProjectileSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            soundManager.CreateSound().WithSoundData(shootProjectileSoundData).WithPosition(transform.position).Play();

        }
    }
    protected override void InitSoundSettings() {
        base.InitSoundSettings();
        impactProjectileSoundDatas.loop = false;
        impactProjectileSoundDatas.frequentSound = true;



        SetMusicClip();
    }
    protected override void SetMusicClip() {
        base.SetMusicClip();
        var projectilesCategory = soundManager.audioGalleryEntries.ProjectilesCategory;
        foreach (var field in projectilesCategory.GetAudioClipFields()) {
            // Matching the name convention for OnHit sounds
            if (field.Name == this.GetType().Name + "OnImpact") {
                // Get the value from the scriptable object field
                AudioClip clip = (AudioClip)field.GetValue(projectilesCategory);
                // Assign it to your local variable
                impactProjectileSoundDatas.clip = clip;
            }
        }


    }
    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void SetDirection(Vector3 dir) {
        inpactPosition = dir;
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }

    private void HandleProjectile() {
        animator.Play("ballisticProjectileMovementAnimation");
        // Calculate the direction vector
        Vector3 direction = (inpactPosition - transform.position).normalized;

        // Set the projectile's rotation to face the direction
        transform.up = inpactPosition;
        transform.DOMove(inpactPosition, 5f).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => {
            animator.Play("ballisticProjectileInpactAnimation");
            inpactCollider.enabled = true;
            if (impactProjectileSoundDatas.clip != null && !GameSettings.Instance.SFXOFF) {
                soundManager.CreateSound().WithSoundData(impactProjectileSoundDatas).WithPosition(transform.position).Play();

            }
        });
    }


}
