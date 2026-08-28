using UnityEngine;
using TMPro;

public class VillageTutorialUI : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;

    private bool movementTutorialDone = false;

    private void Start()
    {
        tutorialText.gameObject.SetActive(true);
        tutorialText.text = "Press WASD to move around";
    }

    // Called by PlayerMovement when player moves
    public void CompleteMovementTutorial()
    {
        if (!movementTutorialDone)
        {
            movementTutorialDone = true;
            tutorialText.gameObject.SetActive(false);
        }
    }

    public void ShowInteractMessage()
    {
        tutorialText.gameObject.SetActive(true);
        tutorialText.text = "Press E to interact";
    }

    public void HideInteractMessage()
    {
        tutorialText.gameObject.SetActive(false);
    }
}