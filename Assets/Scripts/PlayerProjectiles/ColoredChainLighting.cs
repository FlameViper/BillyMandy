using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ColoredChainLighting : MonoBehaviour {


    private Collider2D chainCollider;

    public int damage;

    [SerializeField] private GameObject chainLightning;
    [SerializeField] private GameObject gotBouncedOn;
    public int numberOfBounces;
    public string color;
    [SerializeField] private bool canBounceOnNormalEnemies = true;
    private int alreadyBounced;
    private ParticleSystem particle;
    [SerializeField] private ParticleSystem particleHitEffect;
    private GameObject start;
    private GameObject end;
    private Animator animator;

    private void Start() {
        Destroy(gameObject, 0.2f);
        if (numberOfBounces <= 0) {
            Destroy(gameObject);
        }
        chainCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        particle = GetComponent<ParticleSystem>();

        particle.startColor = GetColor();
        particleHitEffect.startColor = GetColor();
        start = gameObject;
        alreadyBounced = 1;
        canBounceOnNormalEnemies = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision == null) {
            return;
        }
        if (alreadyBounced <= 0) {
          
            return;
        }
        if (canBounceOnNormalEnemies) {
            if (collision.CompareTag("Enemy") || collision.CompareTag("ColoredEnemy")) {
             
                if (collision.GetComponentInChildren<ColoredProjectileGotBouncedOn>()) {

                
                    particle.Stop();
                    return;
                }
             


                end = collision.gameObject;
                numberOfBounces--;
                alreadyBounced--;
                ColoredChainLighting coloredChainLighting = Instantiate(chainLightning, collision.gameObject.transform.position, Quaternion.identity).GetComponent<ColoredChainLighting>();
                coloredChainLighting.numberOfBounces = numberOfBounces;
                Instantiate(gotBouncedOn, collision.gameObject.transform);
                collision.GetComponent<Enemy>().TakeDamage(damage, false);
                animator.StopPlayback();
                chainCollider.enabled = false;
                particle.Play();
                EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = start.transform.position;
                particle.Emit(emitParams, 1);
                emitParams.position = end.transform.position;
                particle.Emit(emitParams, 1);

                particle.Stop();
                Destroy(gameObject, 0.2f);
            }
        }
        else {
         
            if (collision.CompareTag("ColoredEnemy")) {
            

                if (collision.GetComponentInChildren<ColoredProjectileGotBouncedOn>()) {
                
                    particle.Stop();
                    return;
                }

               
                ColoredEnemy coloredEnemy = collision.GetComponent<ColoredEnemy>();
                if (coloredEnemy.currentColor != color) {
                  
                    particle.Stop();
                    return;
                }
            
                coloredEnemy.TakeDamage(20, false);
                end = collision.gameObject;
                numberOfBounces--;
                alreadyBounced--;
                ColoredChainLighting coloredChainLighting = Instantiate(chainLightning, collision.gameObject.transform.position, Quaternion.identity).GetComponent<ColoredChainLighting>();
                coloredChainLighting.numberOfBounces = numberOfBounces;
                Instantiate(gotBouncedOn, collision.gameObject.transform);
                animator.StopPlayback();
                chainCollider.enabled = false;
                particle.Play();
                EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = start.transform.position;
                particle.Emit(emitParams, 1);
                emitParams.position = end.transform.position;
                particle.Emit(emitParams, 1);
                emitParams.position = (start.transform.position + end.transform.position) / 2;
                particle.Emit(emitParams, 1);
                particle.Stop();
                Destroy(gameObject, 0.2f);
            }

        }
    }



    public void SetCanBounceOnNormalEnemies() {
        canBounceOnNormalEnemies = false;
    }


    public Color GetColor() {
        switch (color) {
            case "Red":
                return Color.red;
            case "Green":
                return Color.green;
            case "Yellow":
                return Color.yellow;
            case "Purple":
                return new Color( 143 , 0 , 254, 1 );
            default:
                return Color.red;
        }
    }

}
