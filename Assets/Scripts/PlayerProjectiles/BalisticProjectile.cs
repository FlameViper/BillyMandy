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
    protected override void Awake() {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
    }
    protected override void Start()
    {
        inpactCollider.enabled = false;
        HandleProjectile();
        
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
        });
    }


}
