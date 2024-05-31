using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredEnemy : Enemy {
    [SerializeField]private string color;
    public string currentColor;
    protected override void Start() {
        base.Start();
        currentColor = color;
    }

    protected override void Update() {
        base.Update();

    }


    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") || collision.CompareTag("WarriorGotchi")) {
            potentialTargets.Add(collision.transform);
            UpdateTarget();
        }
        else if (collision.CompareTag("ColoredProjetile")) {
            ColoCheck(collision.GetComponent<ColoredProjectile>().color);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) {
        if (potentialTargets.Contains(collision.transform)) {
            potentialTargets.Remove(collision.transform);
            UpdateTarget();
        }
    }
    public void ColoCheck(string projectileColor) {
       if(projectileColor == currentColor) {

       }
    }
    //public void ColoCheck(string projectileColor) {
    //    switch (currentColor) {
    //        case "Red":
    //            projectileColor = currentColor;
    //            break;
    //        case "Green":

    //            break;
    //        case "Yellow":

    //            break;
    //        case "Purple":

    //            break;
    //        default:
    //            break;
    //    }
    //}
}
