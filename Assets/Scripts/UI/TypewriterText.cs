using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterText : MonoBehaviour
{
    public float typingSpeed = 0.09f;

    //used to assign the next TMP object in the inspector. 
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

        //Hide the next text until  this text is finished 

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
    // Used by DialogueManager to start typing new dialogue
    public void SetText(string newText)
    {
        StopAllCoroutines();

        fullText = newText;
        textComponent.text = "";

        IsTyping = true;
        IsComplete = false;

        // Hide next text if one is assigned
        if (nextText != null)
        {
            nextText.SetActive(false);
        }

        StartCoroutine(TypeText());
    }

    public void FinishTyping()
    {
        if (IsComplete)
            return;

        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            fullText = textComponent.text;
        }

        StopAllCoroutines();

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
