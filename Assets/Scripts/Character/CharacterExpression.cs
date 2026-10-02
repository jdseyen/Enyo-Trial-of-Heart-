using UnityEngine;

public class CharacterExpression : MonoBehaviour
{
    public GameObject[] expressions;

    public void SetExpression(int expressionIndex)
    {
        // Turn off all expressions
        foreach (GameObject expression in expressions)
        {
            if (expression != null)
            {
                expression.SetActive(false);
            }
        }

        // Turn on the selected expression
        if (expressionIndex >= 0 && expressionIndex < expressions.Length)
        {
            expressions[expressionIndex].SetActive(true);
        }
    }
}