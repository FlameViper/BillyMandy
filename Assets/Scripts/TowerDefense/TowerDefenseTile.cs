using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDefenseTile : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler {
    [SerializeField] int wallCoinCost=50;
    [SerializeField] GameObject terrain;
    [SerializeField] GameObject selected;
    private TowerDefenseTower currentTower;
    bool terrainOn;
   
    private void Awake() {
        terrain.SetActive(false);
        selected.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnPointerUp(PointerEventData eventData) {

    }


    public void OnPointerEnter(PointerEventData eventData) {
        if (!TowerDefenseManager.Instance.isInPreparationPhase) {
            return;
        }
        selected.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!TowerDefenseManager.Instance.isInPreparationPhase) {
            return;
        }
        selected.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData) {
        // If not in preparation phase
        //if (!TowerDefenseManager.Instance.isInPreparationPhase) {
        //    HandleTowerPlacement();
        //    return;
        //}

        // Toggle terrain state
        terrainOn = !terrainOn;

        // If terrain is enabled
        if (terrainOn) {
            TryEnableTerrain();
        }
        else {
            HandleTerrainDisable();
        }
    }

    //// Handle tower placement
    //private void HandleTowerPlacement() {
    //    if (TowerDefenseManager.Instance.SelectedTowerIndex != -1 && terrainOn) {
    //        if (currentTower != null) {
    //            currentTower.DestroyTower();
    //        }
    //        if(TowerDefenseManager.Instance.SelectedTowerIndex != -2) {
    //            currentTower = TowerDefenseManager.Instance.PlaceTower(transform).GetComponent<TowerDefenseTower>();

    //        }
    //    }
    //}

    // Try to enable the terrain
    private void TryEnableTerrain() {
        if (!TowerDefenseManager.Instance.isInPreparationPhase) {
            return;
        }
        if (ResourceManager.Instance.Coins >= wallCoinCost/*TowerDefenseManager.Instance.numberOfAvalabileWalls > 0*/) {
           // TowerDefenseManager.Instance.AddWall();
            terrain.SetActive(true);
            ResourceManager.Instance.SubtractCoins(wallCoinCost);
          
        }
        else {
            // Revert terrain state if no available walls
            terrainOn = !terrainOn;
        }
    }

    // Handle terrain disable logic
    private void HandleTerrainDisable() {
        if (TowerDefenseManager.Instance.SelectedTowerIndex == -1) {
            if (!TowerDefenseManager.Instance.isInPreparationPhase) {
                return;
            }
            terrain.SetActive(false);
            ResourceManager.Instance.AddCoins(wallCoinCost);
            //TowerDefenseManager.Instance.RemoveWall();
            return;
        }

        // Re-enable terrain if a tower is selected
        terrainOn = !terrainOn;
        if (TowerDefenseManager.Instance.SelectedTowerIndex != -1 && terrainOn) {
            
            if (currentTower != null) {
                currentTower.DestroyTower();
                if (TowerDefenseManager.Instance.isInPreparationPhase) {
                    ResourceManager.Instance.AddCoins(currentTower.TowerCoinCost);
                    currentTower = null;

                }
            }
            if (TowerDefenseManager.Instance.SelectedTowerIndex != -2) {
                currentTower = TowerDefenseManager.Instance.PlaceTower(transform).GetComponent<TowerDefenseTower>();
                if (ResourceManager.Instance.Coins < currentTower.TowerCoinCost) {
                    currentTower.DestroyTower();
                    return;
                }
                ResourceManager.Instance.SubtractCoins(currentTower.TowerCoinCost);

            }

        }
    }

}
