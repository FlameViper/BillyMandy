using UnityEngine;
using UnityEngine.UI;

public class ShopKeeper : MonoBehaviour
{
    public Text displayText;
    public AudioSource audioSource;
    public string message = "Welcome to my shop!";

    // Start is used to initialize any variables or state before the application starts
    void Start()
    {
        if (displayText != null)
            displayText.text = ""; // Initially clear the text
    }

    // This public method can be linked via the Inspector in Unity
    public void DisplayMessageAndPlaySound()
    {
        // Display the message on the linked Text component
        if (displayText != null)
            displayText.text = message;

        // Play the linked audio source when the method is called
        if (audioSource != null)
            audioSource.Play();
    }
}
