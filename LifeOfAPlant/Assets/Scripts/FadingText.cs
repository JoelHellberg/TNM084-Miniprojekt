using UnityEngine;
using TMPro;
using System.Collections;

public class FadingText : MonoBehaviour
{
    public TMP_Text textMeshPro; // Reference to the TextMeshPro - Text component
    public float fadeDuration = 1.0f; // Duration of the fade in/out effect

    private void Start()
    {
        // Start the fade-in coroutine
        StartCoroutine(FadeInOutRoutine());
    }

    private IEnumerator FadeInOutRoutine()
    {
        // Fade in
        yield return StartCoroutine(FadeTo(1.0f, fadeDuration));

        // Wait for a short period before fading out
        yield return new WaitForSeconds(2.0f); // Adjust the delay time as needed

        // Fade out
        yield return StartCoroutine(FadeTo(0.2f, fadeDuration));

        // Repeat the fade-in/out loop if needed
        StartCoroutine(FadeInOutRoutine());
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = textMeshPro.color.a;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, newAlpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha reaches the target value
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, targetAlpha);
    }
}
