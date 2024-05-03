using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : Projectile
{
    [SerializeField] private float projectileMaxDistance = 6f;
    [SerializeField] private float projectileArc=2f;

    protected override void Awake() {
        base.Awake();
    }


    protected override void Start()
    {
        transform.DORotate(new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360)
           .SetLoops(-1, LoopType.Incremental)
           .SetEase(Ease.Linear);

        // Define the points for the boomerang's path

        Vector3[] pathPoints = new Vector3[] {
            transform.position, // Initial position
            transform.position + direction * 8f,
            //transform.position + new Vector3(direction.x * 6f + 2f,direction.y * 6f),
            //transform.position + new Vector3(direction.x * 3f + 1f,direction.y * 3f),
            transform.position + new Vector3(direction.x * projectileMaxDistance + projectileArc,direction.y * projectileMaxDistance),
            transform.position + new Vector3(direction.x * projectileMaxDistance/2 + projectileArc/2,direction.y * projectileMaxDistance/2),
            transform.position // Return to the initial position below it
        };

        // Move the projectile along the defined path
        transform.DOLocalPath(pathPoints, 1f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                // Once the path is complete, destroy the projectile
                if(gameObject != null) {
                    transform.DOKill();
                    Destroy(gameObject);
                }
            });


    }


    protected override void Update()
    {
        
    }


    //protected override IEnumerator DestroyAfterDelay() {
    //   yield return null;

    //}
}
