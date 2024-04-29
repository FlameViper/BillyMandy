using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float fadeDuration = 1.0f;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0.0f;
        Vector3 startPosition = transform.position;
        Text text = GetComponent<Text>();
        Color originalColor = text.color;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0, t));
            transform.position = startPosition + new Vector3(0, moveSpeed * elapsed, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Destroy the text object after fade is complete
    }
}
