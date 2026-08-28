using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 1.5f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    System.Collections.IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
