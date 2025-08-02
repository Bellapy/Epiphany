
using UnityEngine;

public class DialogueActivationZone : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueToStart;
    [SerializeField] private bool triggerOnce = true;
    private bool hasBeenTriggered = false;
    
    // <<< Removi a referência ao PuzzleManager daqui, ela não é mais necessária >>>

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasBeenTriggered) return;
        if (other.CompareTag("Player"))
        {
            // CHAMADA CORRETA E SIMPLES
            DialogueManager.Instance.StartDialogue(dialogueToStart);
            hasBeenTriggered = true;
        }
    }
}
