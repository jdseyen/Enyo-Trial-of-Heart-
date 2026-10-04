using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour
{
    public GameObject shopBackground;
    public GameObject foodBackground;

    public CanvasGroup fadeOverlay;

    public float fadeDuration = 0.5f;

    public void ChangeToFood()
    {
        StartCoroutine(ChangeBackground(foodBackground));
    }

    public void ChangeToShop()
    {
        StartCoroutine(ChangeBackground(shopBackground));
    }

    private IEnumerator ChangeBackground(GameObject newBackground)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1));

        // Turn off all backgrounds
        shopBackground.SetActive(false);
        foodBackground.SetActive(false);

        // Turn on the new background
        newBackground.SetActive(true);

        // Fade back in
        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeOverlay.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            fadeOverlay.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                time / fadeDuration
            );

            yield return null;
        }

        fadeOverlay.alpha = targetAlpha;
    }
}