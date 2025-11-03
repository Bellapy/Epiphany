// Em Scripts/Player/PlayerInteractor.cs

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractor : MonoBehaviour
{
    private List<IInteractable> interactablesInRange = new List<IInteractable>();

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
            
            // LOG DE CHAMADA
            if (UIManager.Instance != null)
            {
                Debug.Log("<color=lime>[PlayerInteractor] Entrei no trigger. Chamando UIManager.Instance.ShowInteractionPrompt().</color>");
                UIManager.Instance.ShowInteractionPrompt();
            }
            else
            {
                Debug.LogError("<color=red>[PlayerInteractor] Tentei chamar o UIManager, mas UIManager.Instance é NULO!</color>");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactablesInRange.Remove(interactable);
            
            if (interactablesInRange.Count == 0 && UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractionPrompt();
            }
        }
    }
}