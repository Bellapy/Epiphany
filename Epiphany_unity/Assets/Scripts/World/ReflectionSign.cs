using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReflectionSign : MonoBehaviour, IInteractable
{
    [Header("Dados da Reflexão")]
    [SerializeField] private ReflectionData reflectionData;

    public void Interact()
    {
        if (reflectionData != null && DialogueManager.Instance != null)
        {
            Debug.Log($"Placa '{gameObject.name}' interagida. Chamando StartReflection.");
            
            // <<< A CHAMADA CORRETA >>>
            DialogueManager.Instance.StartReflection(reflectionData);
        }
        else
        {
            Debug.LogWarning($"Placa '{gameObject.name}' não tem ReflectionData ou DialogueManager não foi encontrado.");
        }
    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}