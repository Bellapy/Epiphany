using UnityEngine;
using UnityEngine.Events; // Necessário para usar UnityEvent

[RequireComponent(typeof(Collider2D))]
public class EventTriggerZone : MonoBehaviour
{
    [Header("Configuração de Eventos")]
    [Tooltip("Um evento customizável que será disparado quando o jogador entrar na zona.")]
    public UnityEvent onTriggerEnterEvent; // <<< EVENTO GENÉRICO ADICIONADO AQUI

    // Mantive os campos antigos caso você queira usá-los como atalhos no futuro,
    // mas a lógica principal usará o UnityEvent.
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
            Debug.Log($"[EventTriggerZone] Jogador entrou na zona '{gameObject.name}'. Disparando eventos.");
            
            // --- LÓGICA ATUALIZADA ---

            // 1. Invoca o evento genérico. É aqui que vamos conectar a música e o jogador.
            onTriggerEnterEvent.Invoke();

            // 2. A lógica antiga ainda funciona como um bônus.
            // Dispara a Animação (se houver)
            if (targetAnimator != null)
            {
                targetAnimator.SetTrigger(triggerName);
                Debug.Log($"Evento: Animação '{triggerName}' disparada.");
            }

            // Mostra a Reflexão (se houver)
            if (reflectionData != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartReflection(reflectionData);
            }
            
            // 3. Marca como usado depois que todas as ações foram disparadas
            hasBeenTriggered = true;
        }
    }
}