using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReflectionZone : MonoBehaviour
{
    [SerializeField] private ReflectionData reflectionData;
    [SerializeField] private bool triggerOnce = true;

    [Header("Feedback Visual")]
    [SerializeField] private SpriteRenderer visualFeedback;
    [SerializeField] private Color activatedColor = Color.white;
    private Color originalColor;
    
    private bool hasBeenTriggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (visualFeedback != null)
        {
            originalColor = visualFeedback.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasBeenTriggered) return;

        if (other.CompareTag("Player"))
        {
            if (DialogueManager.Instance != null && reflectionData != null)
            {
                // <<< A CHAMADA FOI CORRIGIDA PARA FALAR DIRETO COM O DIALOGUEMANAGER >>>
                // A lógica de 'requestAccepted' foi removida para simplificar,
                // pois agora o próprio DialogueManager lida com a sobreposição.
                
                DialogueManager.Instance.StartReflection(reflectionData);

                if (visualFeedback != null)
                {
                    visualFeedback.color = activatedColor;
                }
                hasBeenTriggered = true;
            }
        }
    }
}