using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TypewriterText dialogueText;
    public TMP_Text characterName;
    public Button nextButton;

    private int currentLine = 0;

    private string[] dialogue =
    {
        "Hello there! Would you like to try one of our handmade puzzles?",
        "They're all made right here in our little shop.",
        "My mother taught me how to make them."
    };

    private string[] speakers =
    {
        "ENYO",
        "ENYO",
        "ENYO"
    };

    private void Start()
    {
        ShowLine();

        nextButton.onClick.AddListener(NextDialogue);
    }

    private void ShowLine()
    {
        characterName.text = speakers[currentLine];

        dialogueText.SetText(dialogue[currentLine]);
    }

    private void NextDialogue()
    {
        // First click: finish the typewriter
        if (dialogueText.IsTyping)
        {
            dialogueText.FinishTyping();
            return;
        }

        // Second click: move to the next line
        if (currentLine < dialogue.Length - 1)
        {
            currentLine++;
            ShowLine();
        }
    }
}