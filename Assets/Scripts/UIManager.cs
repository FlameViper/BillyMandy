using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera battleCamera;
    public Camera upgradesCamera;
    public Camera example2Camera;


    void Start()
    {
        EnableMainCamera();
    }


    public void EnableBattleCamera()
    {
        mainCamera.enabled = false;
        battleCamera.enabled = true;
        upgradesCamera.enabled = false;
        example2Camera.enabled = false;
    }

    public void EnableMainCamera()
    {
        mainCamera.enabled = true;
        battleCamera.enabled = false;
        upgradesCamera.enabled = false;
        example2Camera.enabled = false;
    }

    public void EnableUpgradesCamera()
    {
        mainCamera.enabled = false;
        battleCamera.enabled = false;
        upgradesCamera.enabled = true;
        example2Camera.enabled = false;
    }

    public void EnableExample2Camera()
    {
        mainCamera.enabled = false;
        battleCamera.enabled = false;
        upgradesCamera.enabled = false;
        example2Camera.enabled = true;
    }
}
