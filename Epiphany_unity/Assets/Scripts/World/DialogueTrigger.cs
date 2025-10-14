using UnityEngine;


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


    public void Interact()
    {
   
        if (triggerOnce && hasBeenTriggered) return;
        
        if (DialogueManager.Instance != null && dialogueToStart != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToStart); 
            hasBeenTriggered = true;
        }
    }
}