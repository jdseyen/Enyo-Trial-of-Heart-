using UnityEngine;

// All possible speakers in the game
public enum SpeakerType
{
    Narration,
    Enyo,
    Mother,
    Customer,
    MadameBlanche,
    MysteriousCreature,
    Roberto,
    LuangPhor,
    Moy,
    Victer
}

// Enyo's available expressions
public enum EnyoExpressionType
{
    Neutral,
    Laughing,
    Smiling,
    Sad,
    Worried,
    Upset
}

// Actions that a character can perform on a dialogue line
public enum CharacterAction
{
    None,
    Enter,
    Exit
}

// Backgrounds that can be selected for each dialogue line
public enum BackgroundType
{
    None,
    Shop,
    Food
}

// Stores expression information for a character on a dialogue line
[System.Serializable]
public class CharacterExpressionData
{
    // Which character should change their expression
    public SpeakerType character;

    // Which expression should be shown
    // Example:
    // 0 = Neutral
    // 1 = Laughing
    // 2 = Smiling
    // 3 = Sad
    // 4 = Worried
    // 5 = Upset
    public int expressionIndex;
}

// Stores all information needed for one dialogue line
[System.Serializable]
public class DialogueLine
{
    // The actual dialogue text
    [TextArea(2, 5)]
    public string dialogue;

    // Who is speaking this line
    public SpeakerType speaker;

    // Whether the speaker enters, exits, or does nothing
    public CharacterAction characterAction;

    // Which background should be shown for this dialogue line
    // Choose Shop, Food, or None in the Inspector
    public BackgroundType background;

    // Expressions for any characters present during this line
    public CharacterExpressionData[] characterExpressions;
}