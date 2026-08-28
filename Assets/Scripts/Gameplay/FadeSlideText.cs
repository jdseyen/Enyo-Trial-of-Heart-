
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class FadeSlideText : MonoBehaviour
{
    public float duration = 0.5f; 
    public Vector3 slideOffset = new Vector3(-30f,0f,0); //from side
    public GameObject nextText; //optional chaining

    TextMeshProUGUI text;
    Vector3 endPos;
    Color originalColor;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        endPos = transform.localPosition;
        originalColor = text.color; 
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Play()); 
    }

    IEnumerator Play()
    {
        float t = 0f;
        transform.localPosition = endPos + slideOffset; 
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = Mathf.SmoothStep(0, 1, t / duration);

            transform.localPosition = Vector3.Lerp(endPos + slideOffset, endPos, eased); 
            text.color = new Color(originalColor.r, originalColor.g,originalColor.b,eased);

            yield return null;
        }

        transform.localPosition = endPos;
        text.color = originalColor;

        if (nextText != null)
        {
            nextText.SetActive(true);
        }
           
    }
}
