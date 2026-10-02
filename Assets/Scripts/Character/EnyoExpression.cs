using UnityEngine;

public class EnyoExpression : MonoBehaviour
{
    public GameObject neutral;
    public GameObject laughing;
    public GameObject smiling;
    public GameObject sad1;
    public GameObject sad2;
    public GameObject sad3;


    public void SetExpression(EnyoExpressionType expression)
    {
        // Turn all expressions off
        neutral.SetActive(false);
        laughing.SetActive(false);
        smiling.SetActive(false);
        sad1.SetActive(false);
        sad2.SetActive(false);
        sad3.SetActive(false);

        // Turn on the selected expression
        switch (expression)
        {
            case EnyoExpressionType.Neutral:
                neutral.SetActive(true);
                break;

            case EnyoExpressionType.Laughing:
                laughing.SetActive(true);
                break;

            case EnyoExpressionType.Smiling:
                smiling.SetActive(true);
                break;

            case EnyoExpressionType.Sad1:
                sad1.SetActive(true);
                break;

            case EnyoExpressionType.Sad2:
                sad2.SetActive(true);
                break;

            case EnyoExpressionType.Sad3:
                sad3.SetActive(true);
                break;
        }
    }
}