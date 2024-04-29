using UnityEngine;
using System.Collections; // Required for Coroutines

public class DifficultySelectionHandler : MonoBehaviour
{
    public GameSettings gameSettings;
    public UIManager uiManager; // Reference to your UIManager

    public void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                GameSettings.Instance.currentDifficulty = GameSettings.Difficulty.Easy;
                break;
            case "Medium":
                GameSettings.Instance.currentDifficulty = GameSettings.Difficulty.Medium;
                break;
            case "Hard":
                GameSettings.Instance.currentDifficulty = GameSettings.Difficulty.Hard;
                break;
        }

        StartCoroutine(DelayCameraSwitch()); // Start the coroutine to delay camera switch
    }

    private IEnumerator DelayCameraSwitch()
    {
        yield return new WaitForSeconds(1); // Wait for one second
        uiManager.EnableUpgradesCamera(); // Switch to upgrades camera after the delay
    }
}
