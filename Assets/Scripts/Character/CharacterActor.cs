using UnityEngine;

public class CharacterActor : MonoBehaviour
{
    public SpeakerType speaker;
    public CharacterExpression characterExpression;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetExpression(int expressionIndex)
    {
        if (characterExpression != null)
        {
            characterExpression.SetExpression(expressionIndex);
        }
    }
}