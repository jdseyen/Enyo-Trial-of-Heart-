using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TypewriterText dialogueText;
    public TMP_Text characterName;
    public Button nextButton;

    // All characters in this scene
    public CharacterActor[] characters;

    // Dialogue lines shown in the Unity Inspector
    public DialogueLine[] dialogueLines;

    private int currentLine = 0;

    private void Start()
    {
        ShowLine();
        nextButton.onClick.AddListener(NextDialogue);
    }

    private void ShowLine()
    {
        DialogueLine line = dialogueLines[currentLine];

        // Handle character entering or leaving
        HandleCharacterAction(line);

        // Show the speaker's name
        characterName.text = GetSpeakerName(line.speaker);

        // Show the dialogue text
        dialogueText.SetText(line.dialogue);

        // Set expressions for all characters on this line
        SetCharacterExpressions(line);
    }

    private void SetCharacterExpressions(DialogueLine line)
    {
        if (line.characterExpressions == null)
            return;

        foreach (CharacterExpressionData expressionData in line.characterExpressions)
        {
            CharacterActor character = GetCharacter(expressionData.character);

            if (character != null)
            {
                character.SetExpression(expressionData.expressionIndex);
            }
        }
    }

    private CharacterActor GetCharacter(SpeakerType speaker)
    {
        foreach (CharacterActor character in characters)
        {
            if (character != null && character.speaker == speaker)
            {
                return character;
            }
        }

        return null;
    }

    private void HandleCharacterAction(DialogueLine line)
    {
        CharacterActor character = GetCharacter(line.speaker);

        if (character == null)
        {
            Debug.LogWarning("Could not find character: " + line.speaker);
            return;
        }

        if (line.characterAction == CharacterAction.Enter)
        {
            character.Show();
        }
        else if (line.characterAction == CharacterAction.Exit)
        {
            character.Hide();
        }
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
        if (currentLine < dialogueLines.Length - 1)
        {
            currentLine++;
            ShowLine();
        }
    }

    private string GetSpeakerName(SpeakerType speaker)
    {
        switch (speaker)
        {
            case SpeakerType.Enyo:
                return "ENYO";

            case SpeakerType.Mother:
                return "MOTHER";

            case SpeakerType.MadameBlanche:
                return "MADAME BLANCHE";

            case SpeakerType.MysteriousCreature:
                return "MYSTERIOUS CREATURE";

            case SpeakerType.Roberto:
                return "ROBERTO";

            case SpeakerType.LuangPhor:
                return "LUANG PHOR";

            case SpeakerType.Moy:
                return "MOY";

            case SpeakerType.Victer:
                return "VICTER";

            case SpeakerType.Customer:
                return "CUSTOMER";

            default:
                return "";
        }
    }
}