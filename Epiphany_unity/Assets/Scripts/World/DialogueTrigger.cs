using UnityEngine;

// <<< MUDANÇA 1: Adicione ", IInteractable" aqui >>>
// Isso diz ao C# que este script "promete" ter uma função Interact().
[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour, IInteractable 
{
    [SerializeField] private DialogueData dialogueToStart;
    [SerializeField] private bool triggerOnce = true;
    private bool hasBeenTriggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    // O OnTriggerEnter2D original foi removido, pois agora a interação é manual.
    // Se você ainda quiser que ele funcione ao entrar na área E ao interagir, podemos mantê-lo.
    // Mas para o que você descreveu, a interação manual é o ideal.

    // <<< MUDANÇA 2: Adicione o método Interact() >>>
    // Este é o método que o PlayerInteractor vai chamar quando você apertar a tecla "E".
    public void Interact()
    {
        // A mesma lógica que estava no OnTriggerEnter2D agora vive aqui.
        if (triggerOnce && hasBeenTriggered) return;
        
        if (DialogueManager.Instance != null && dialogueToStart != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToStart); 
            hasBeenTriggered = true;
        }
    }
}