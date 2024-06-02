using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredExplosion : MonoBehaviour {
    Animator animator;
    [SerializeField] private string color;
    [SerializeField] private float destroyDelay = 0.5f;

    private int numberOfColoredEnemies;
    bool exploded;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        PlayColoredExplosion();
        StartCoroutine(DestroyAfterDelay());
    }
    private void Update() {

        //if (numberOfColoredEnemies >= 3 && !exploded) {
        //    exploded = true;
        //    PlayColoredExplosion();
            
        //}

    }
    private void OnTriggerEnter2D(Collider2D collision) {
        //if (collision.CompareTag("ColoredEnemy") && !exploded) {
        //    ColoredEnemy coloredEnemy = collision.gameObject.GetComponent<ColoredEnemy>();
        //    if (coloredEnemy.currentColor == color) {
        //        numberOfColoredEnemies++;
        //    }

        //}
        if (collision == null) {
            return;
        }
        if (collision.CompareTag("Enemy")  /*&& exploded*/) {
            //if its explosion dmg it dosent triggr others explosions
            collision.gameObject.GetComponent<Enemy>().TakeDamage(200, true);


        }
    }

    //private void OnTriggerStay2D(Collider2D collision) {
    //    if (collision == null) {
    //        return;
    //    }
    //    if (collision.CompareTag("ColoredEnemy") /*&& exploded*/) {
    //        //if its explosion dmg it dosent triggr others explosions
    //        collision.gameObject.GetComponent<Enemy>().TakeDamage(200,false);
          

    //    }
    //}

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
