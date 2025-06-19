// ReflectionSign.cs
using UnityEngine;

// [RequireComponent] garante que o objeto sempre terá um Collider2D.
[RequireComponent(typeof(Collider2D))]
public class ReflectionSign : MonoBehaviour, IInteractable // <-- Veja, ele implementa o "contrato" IInteractable
{
    [Header("Dados da Reflexão")]
    [SerializeField] private ReflectionData reflectionData;

    // Este é o método que o "contrato" IInteractable nos obriga a ter.
    // Ele será chamado pelo PlayerInteractor quando o jogador apertar 'E'.
    public void Interact()
    {
        if (reflectionData != null && UIManager.Instance != null)
        {
            Debug.Log($"Placa '{gameObject.name}' interagida. Mostrando reflexão.");
            UIManager.Instance.ShowReflection(reflectionData);
        }
        else
        {
            Debug.LogWarning($"Placa '{gameObject.name}' não tem ReflectionData ou UIManager não foi encontrado.");
        }
    }

    private void Awake()
    {
        // Garante que o collider não seja um trigger, ele precisa ser sólido
        // para que o círculo de interação do jogador possa detectá-lo.
        // Se você quiser que o jogador possa atravessar a placa, deixe como trigger.
        // Para uma placa, sólido (isTrigger = false) geralmente faz mais sentido.
        GetComponent<Collider2D>().isTrigger = true;
    }
}