using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Referências de Sistema")]
    [SerializeField] private UIManager uiManager;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    
    void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && interactablesInRange.Count > 0)
        {
            interactablesInRange[interactablesInRange.Count - 1].Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactablesInRange.Add(interactable);
            if (uiManager != null)
            {
                uiManager.ShowInteractionPrompt();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactablesInRange.Remove(interactable);
            
            if (interactablesInRange.Count == 0 && uiManager != null)
            {
                uiManager.HideInteractionPrompt();
            }
        }
    }
}