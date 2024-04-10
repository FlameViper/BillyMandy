using UnityEngine;

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

        // Assuming uiManager is assigned in the inspector.
        uiManager.EnableUpgradesCamera(); // Switch to upgrades camera after setting difficulty
    }
}
