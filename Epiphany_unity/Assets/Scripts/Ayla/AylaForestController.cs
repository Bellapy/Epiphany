using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AylaForestController : MonoBehaviour, IInteractable
{
    private enum State
    {
        WaitingForPlayer,
        DialogueInProgress,
        ReadyForHands,
        SequenceComplete
    }

    [Header("Referências")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator aylaAnimator;

    [Header("Configuração da Interação")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private float interactionDistance = 2.0f;

    [Header("Eventos")]
    [Tooltip("Disparado quando a sequência de 'mãos dadas' deve começar.")]
    public UnityEvent OnReadyForHandsSequence;

    private State currentState = State.WaitingForPlayer;
    private bool playerInRange = false;

    void Awake()
    {
        if (aylaAnimator == null)
        {
            aylaAnimator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (currentState == State.SequenceComplete || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        playerInRange = distance < interactionDistance;

        switch (currentState)
        {
            case State.WaitingForPlayer:
                // Se o jogador se aproxima, Ayla se vira para ele e fica pronta para interagir.
                if (playerInRange)
                {
                    // Adicione aqui a lógica para Ayla se virar para o jogador, se necessário.
                    // Ex: spriteRenderer.flipX = playerTransform.position.x < transform.position.x;
                }
                break;

            case State.ReadyForHands:
                // Se o jogador se aproxima novamente, a sequência final começa.
                if (playerInRange)
                {
                    currentState = State.SequenceComplete;
                    OnReadyForHandsSequence.Invoke();
                }
                break;
        }
    }

    // Este método é chamado pelo PlayerInteractor quando o jogador aperta 'E' perto da Ayla.
    public void Interact()
    {
        if (playerInRange && currentState == State.WaitingForPlayer)
        {
            currentState = State.DialogueInProgress;
            DialogueManager.Instance.StartDialogue(dialogueData);
            // Se inscreve no evento de fim de diálogo para saber quando a conversa terminou.
            DialogueManager.OnDialogueEnd += HandleDialogueEnd;
        }
    }

    // Este método é chamado automaticamente quando o diálogo termina.
    private void HandleDialogueEnd()
    {
        // Se desinscreve do evento para não ser chamado novamente por outros diálogos.
        DialogueManager.OnDialogueEnd -= HandleDialogueEnd;
        currentState = State.ReadyForHands;
    }
}