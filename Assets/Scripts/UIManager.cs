using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera battleCamera;
    public Camera upgradesCamera;
    public Camera example2Camera;

    public Canvas mainCanvas;
    public Canvas battleCanvas;
    public Canvas upgradesCanvas;
    public Canvas example2Canvas;

    void Start()
    {
        EnableMainCamera();
    }

    public void EnableBattleCamera()
    {
        SetCameraActive(mainCamera, false, mainCanvas);
        SetCameraActive(battleCamera, true, battleCanvas);
        SetCameraActive(upgradesCamera, false, upgradesCanvas);
        SetCameraActive(example2Camera, false, example2Canvas);
    }

    public void EnableMainCamera()
    {
        SetCameraActive(mainCamera, true, mainCanvas);
        SetCameraActive(battleCamera, false, battleCanvas);
        SetCameraActive(upgradesCamera, false, upgradesCanvas);
        SetCameraActive(example2Camera, false, example2Canvas);
    }

    public void EnableUpgradesCamera()
    {
        SetCameraActive(mainCamera, false, mainCanvas);
        SetCameraActive(battleCamera, false, battleCanvas);
        SetCameraActive(upgradesCamera, true, upgradesCanvas);
        SetCameraActive(example2Camera, false, example2Canvas);
    }

    public void EnableExample2Camera()
    {
        SetCameraActive(mainCamera, false, mainCanvas);
        SetCameraActive(battleCamera, false, battleCanvas);
        SetCameraActive(upgradesCamera, false, upgradesCanvas);
        SetCameraActive(example2Camera, true, example2Canvas);
    }

    private void SetCameraActive(Camera camera, bool isActive, Canvas canvas)
    {
        camera.enabled = isActive;
        canvas.gameObject.SetActive(isActive);
    }
}
