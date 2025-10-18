using UnityEngine;
using UnityEngine.Events; // Necessário para UnityEvent

// Este é um script genérico que pode ser usado para qualquer interação
// que precise apenas chamar um evento no Inspector.
public class InteractionRelay : MonoBehaviour, IInteractable
{
    // Este evento aparecerá no Inspector, e podemos conectar qualquer
    // função pública a ele, como a função de deitar no nosso controller.
    public UnityEvent OnInteract;

    public void Interact()
    {
        // Quando o jogador interage, simplesmente invocamos o evento.
        OnInteract?.Invoke();
        
        // Opcional: Desativa o próprio objeto para que não possa ser usado de novo.
        gameObject.SetActive(false);
    }
}