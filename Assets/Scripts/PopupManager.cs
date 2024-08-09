using UnityEngine;
using UnityEngine.UI;

public class PopupManager : PersistentSingleton<PopupManager> {
    public GameObject confirmationPopup; // The popup UI panel
    public Button yesButton;             // Yes button in the popup
    public Button noButton;              // No button in the popup
    private System.Action onConfirmAction;

    private void Start() {
        // Hide the popup at the start
        confirmationPopup.SetActive(false);

        // Assign listeners to buttons
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
    }

    public void ShowConfirmationPopup(System.Action onConfirm) {
        // Store the action to be executed on confirmation
        onConfirmAction = onConfirm;

        // Show the popup
        confirmationPopup.SetActive(true);
    }

    private void OnYesButtonClicked() {
        // Execute the stored action if Yes is clicked
        onConfirmAction?.Invoke();
        // Hide the popup
        confirmationPopup.SetActive(false);
    }

    private void OnNoButtonClicked() {
        // Just hide the popup if No is clicked
        confirmationPopup.SetActive(false);
    }
}
