using TMPro;
using UnityEngine;

public class TrialDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject choicePanel;

    [TextArea(3, 5)]
    public string[] dialogueLines;

    private int currentLine = 0;

    void OnEnable()
    {
        currentLine = 0;
        dialogueText.text = dialogueLines[currentLine];
    }

    public void NextLine()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            gameObject.SetActive(false);

            choicePanel.SetActive(true);
        }
    }
}