using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueActivationZone : MonoBehaviour
{
    [Header("Configuração do Diálogo")]
    [SerializeField] private DialogueData dialogueToStart;
    [SerializeField] private bool triggerOnce = true;
    
    private bool hasBeenTriggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasBeenTriggered) return;

        if (other.CompareTag("Player"))
        {
            if (DialogueManager.Instance != null && dialogueToStart != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueToStart);
                hasBeenTriggered = true;
            }
        }
    }
}