using UnityEngine;
using UnityEngine.Events; 

[RequireComponent(typeof(Collider2D))]
public class EventTriggerZone : MonoBehaviour
{
    [Header("Configuração de Eventos")]
    [Tooltip("Um evento customizável que será disparado quando o jogador entrar na zona.")]
    public UnityEvent onTriggerEnterEvent; 

    [SerializeField] private Animator targetAnimator;
    [SerializeField] private string triggerName = "Activate";
    [SerializeField] private ReflectionData reflectionData;

    [Header("Controle do Gatilho")]
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
            onTriggerEnterEvent.Invoke();

            if (targetAnimator != null)
            {
                targetAnimator.SetTrigger(triggerName);
            }

            if (reflectionData != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartReflection(reflectionData);
            }
            
            hasBeenTriggered = true;
        }
    }
}