using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PersonalReflectionTrigger : MonoBehaviour, IInteractable
{
    [Header("Dados da Reflexão Pessoal")]
    [SerializeField] private ReflectionData reflectionData;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact()
    {
        // A verificação agora é pelo DialogueManager, não pelo UIManager
        if (reflectionData != null && DialogueManager.Instance != null)
        {
            Debug.Log($"Gatilho pessoal '{gameObject.name}' interagido. Mostrando reflexão.");
            
            // <<< A CHAMADA FOI CORRIGIDA PARA FALAR DIRETO COM O DIALOGUEMANAGER >>>
            DialogueManager.Instance.StartReflection(reflectionData);
        }
        else
        {
            Debug.LogWarning($"Gatilho '{gameObject.name}' não tem ReflectionData ou DialogueManager não foi encontrado.");
        }
    }
}