using UnityEngine;

public enum SpeakerType
{
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

public enum EnyoExpressionType
{
    Neutral,
    Laughing,
    Smiling,
    Sad1,
    Sad2,
    Sad3
}

public enum CharacterAction
{
    None,
    Enter,
    Exit
}

[System.Serializable]
public class CharacterExpressionData
{
    public SpeakerType character;

    public int expressionIndex;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string dialogue;

    public SpeakerType speaker;

    public CharacterAction characterAction;

    // Expressions for any characters present in this line
    public CharacterExpressionData[] characterExpressions;
}