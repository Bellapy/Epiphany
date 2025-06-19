// PlayerInteractor.cs
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractor : MonoBehaviour
{
    // Uma lista para guardar todos os interativos que estão ao alcance.
    private List<IInteractable> interactablesInRange = new List<IInteractable>();

    // Este método será chamado pelo componente Player Input quando a ação "Interact" for pressionada.
    public void OnInteract(InputValue value)
    {
        // Se a tecla foi pressionada (não solta) e temos algo para interagir...
        if (value.isPressed && interactablesInRange.Count > 0)
        {
            // Pega o interativo mais próximo (o último que entrou na lista) e chama seu método Interact().
            interactablesInRange[interactablesInRange.Count - 1].Interact();
        }
    }

    // Chamado quando algo entra no nosso trigger de interação.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta pegar o componente que implementa a interface IInteractable.
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Se encontrou, adiciona na lista.
            interactablesInRange.Add(interactable);
            Debug.Log($"Entrou no alcance de: {other.name}");
        }
    }

    // Chamado quando algo sai do nosso trigger de interação.
    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Se saiu, remove da lista.
            interactablesInRange.Remove(interactable);
            Debug.Log($"Saiu do alcance de: {other.name}");
        }
    }
}

// Uma "interface" é como um contrato. Qualquer script que usar IInteractable
// É OBRIGADO a ter um método chamado Interact().
public interface IInteractable
{
    void Interact();
}