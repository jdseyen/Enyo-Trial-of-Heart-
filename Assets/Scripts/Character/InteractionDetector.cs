using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;

    [Header("Interaction")]
    public GameObject interactionIcon;

    [Header("Tutorial UI")]
    public GameObject instructionPanel;
    public TextMeshProUGUI tutorialText;

    private bool movementTutorialDone = false;
    private bool interactionTutorialDone = false;

    private void Start()
    {
        // Hide interaction icon
        interactionIcon.SetActive(false);

        // Show movement tutorial
        instructionPanel.SetActive(true);
        tutorialText.text = "Press WASD to move around";
    }

    public void MarkMovementTutorialComplete()
    {
        if (!movementTutorialDone)
        {
            Debug.Log("Movement tutorial completed!");

            movementTutorialDone = true;
            instructionPanel.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();

            if (!interactionTutorialDone)
            {
                Debug.Log("Interaction tutorial completed!");

                interactionTutorialDone = true;
                instructionPanel.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered trigger with: " + collision.name);

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            Debug.Log("Found IInteractable on: " + collision.name);

            // Complete movement tutorial automatically
            if (!movementTutorialDone)
            {
                movementTutorialDone = true;
            }

            interactableInRange = interactable;

            interactionIcon.SetActive(true);

            if (!interactionTutorialDone)
            {
                Debug.Log("Showing Press E tutorial");

                instructionPanel.SetActive(true);
                tutorialText.text = "Press E to interact";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) &&
            interactable == interactableInRange)
        {
            Debug.Log("Left interaction range");

            interactableInRange = null;

            interactionIcon.SetActive(false);

            if (!interactionTutorialDone)
            {
                instructionPanel.SetActive(false);
            }
        }
    }
}