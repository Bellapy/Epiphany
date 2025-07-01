// Em PlayerInteractor.cs
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractor : MonoBehaviour
{
    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    
    // Chamado pelo componente Player Input quando a tecla "E" é pressionada
    public void OnInteract(InputValue value)
    {
        if (value.isPressed && interactablesInRange.Count > 0)
        {
            // Interage com o último objeto que entrou no alcance
            interactablesInRange[interactablesInRange.Count - 1].Interact();
        }
    }

    // Chamado quando o círculo de alcance do jogador entra em um trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Adiciona o objeto à lista de interativos
            interactablesInRange.Add(interactable);
            
            // Se esta é a primeira interação no alcance, manda o UIManager mostrar o prompt
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInteractionPrompt();
            }
        }
    }

    // Chamado quando o círculo de alcance do jogador sai de um trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Remove o objeto da lista
            interactablesInRange.Remove(interactable);
            
            // Se a lista de interativos ficou vazia, manda o UIManager esconder o prompt
            if (interactablesInRange.Count == 0 && UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractionPrompt();
            }
        }
    }
}