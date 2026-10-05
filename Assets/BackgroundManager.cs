using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour
{
    public GameObject shopBackground;
    public GameObject foodBackground;

    public CanvasGroup fadeOverlay;

    public float fadeDuration = 0.5f;

    private BackgroundType currentBackground = BackgroundType.None;

    public void ChangeToFood(bool shouldFade)
    {
        if (currentBackground == BackgroundType.Food)
            return;

        StartCoroutine(ChangeBackground(
            foodBackground,
            BackgroundType.Food,
            shouldFade
        ));
    }

    public void ChangeToShop(bool shouldFade)
    {
        if (currentBackground == BackgroundType.Shop)
            return;

        StartCoroutine(ChangeBackground(
            shopBackground,
            BackgroundType.Shop,
            shouldFade
        ));
    }

    private IEnumerator ChangeBackground(
        GameObject newBackground,
        BackgroundType newBackgroundType,
        bool shouldFade)
    {
        // Only fade if this dialogue line has Fade Background checked
        if (shouldFade)
        {
            yield return StartCoroutine(Fade(1));
        }

        // Turn off old backgrounds
        shopBackground.SetActive(false);
        foodBackground.SetActive(false);

        // Turn on new background
        newBackground.SetActive(true);

        currentBackground = newBackgroundType;

        // Only fade back in if we faded to black
        if (shouldFade)
        {
            yield return StartCoroutine(Fade(0));
        }
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