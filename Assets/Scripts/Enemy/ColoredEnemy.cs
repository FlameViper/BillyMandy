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



    public void ColoCheck() {
        switch (currentColor) {
            case "Red":

                break;
            case "Green":

                break;
            case "Yellow":

                break;
            case "Purple":

                break;
            default:
                break;
        }
    }
}
