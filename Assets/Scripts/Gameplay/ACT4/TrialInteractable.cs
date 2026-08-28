using UnityEngine;

public class TrialInteractable : MonoBehaviour, IInteractable
{
    public GameObject dialoguePanel;
    public GameObject interactionPanel;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        dialoguePanel.SetActive(true);

        interactionPanel.SetActive(false);
    }
}