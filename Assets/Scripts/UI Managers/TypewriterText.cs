using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterText : MonoBehaviour
{
    public float typingSpeed = 0.09f;

    // KEEP THIS!
    // Assign the next TextMeshPro object in the Inspector.
    public GameObject nextText;

    private TextMeshProUGUI textComponent;
    private string fullText;

    public bool IsTyping { get; private set; }
    public bool IsComplete { get; private set; }

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        fullText = textComponent.text;
        textComponent.text = "";

        IsComplete = false;
    }

    void OnEnable()
    {
        StopAllCoroutines();

        textComponent.text = "";
        IsTyping = true;
        IsComplete = false;

        // Hide the next text until this text is finished
        if (nextText != null)
        {
            nextText.SetActive(false);
        }

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char letter in fullText)
        {
            textComponent.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        CompleteText();
    }

    public void FinishTyping()
    {
        if (!IsTyping)
            return;

        StopAllCoroutines();

        // Instantly finish THIS text
        textComponent.text = fullText;

        CompleteText();
    }

    void CompleteText()
    {
        IsTyping = false;
        IsComplete = true;

        // Activate the next text in your chain
        if (nextText != null)
        {
            nextText.SetActive(true);
        }
    }
}