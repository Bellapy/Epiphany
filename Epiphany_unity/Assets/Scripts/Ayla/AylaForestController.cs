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
                if (playerInRange)
                {
                }
                break;

            case State.ReadyForHands:
                if (playerInRange)
                {
                    currentState = State.SequenceComplete;
                    OnReadyForHandsSequence.Invoke();
                }
                break;
        }
    }

    public void Interact()
    {
        if (playerInRange && currentState == State.WaitingForPlayer)
        {
            if (DialogueManager.Instance == null) return;

            currentState = State.DialogueInProgress;
            
            DialogueManager.Instance.StartDialogue(dialogueData);
            DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
        }
    }

    private void HandleDialogueEnd()
    {
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        currentState = State.ReadyForHands;
    }
}