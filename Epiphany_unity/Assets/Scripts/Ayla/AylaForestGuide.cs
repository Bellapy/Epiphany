using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AylaForestGuide : MonoBehaviour
{
    [Header("Identificador de Estado")]
    [Tooltip("Um nome único para este evento. Usado para salvar e saber se já foi concluído.")]
    [SerializeField] private string tourCompletionFlag = "AylaForestTourCompleted";

    [Header("Alvo e Caminho")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> waypoints;

    [Header("Configuração de Movimento")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float maxDistanceToPlayer = 4.0f;
    [SerializeField] private float resumeDistanceToPlayer = 3.0f;

    [Header("Configuração do Diálogo")]
    [Tooltip("O diálogo que Ayla falará durante o percurso.")]
    [SerializeField] private DialogueData tourDialogue;
    [Tooltip("O índice do waypoint que, ao ser alcançado, iniciará o diálogo (ex: 0 para o primeiro, 1 para o segundo).")]
    [SerializeField] private int dialogueTriggerWaypointIndex = 1;
    
    [Header("Configuração Final")]
    [Tooltip("A porta ou gatilho de transição que será ativado no final do tour.")]
    [SerializeField] private SceneTransitionTrigger finalTransitionTrigger;

    [Header("Referências de Componentes")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Variáveis de controle interno
    private int currentWaypointIndex = 0;
    private bool isWaitingForPlayer = false;
    private bool isTourActive = false;
    private bool dialogueTriggered = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // --- LÓGICA DE PERSISTÊNCIA ---
        // Verifica se o tour já foi concluído em uma sessão anterior.
        if (PlayerPrefs.GetInt(tourCompletionFlag, 0) == 1)
        {
            Debug.Log($"[AylaForestGuide] Tour '{tourCompletionFlag}' já concluído. Desativando Ayla.");
            gameObject.SetActive(false); // Se já foi feito, Ayla nem aparece na cena.
            return;
        }
        
        // Garante que a porta de transição comece trancada se o tour ainda não foi feito.
        if (finalTransitionTrigger != null)
        {
            // Assumindo que o SceneTransitionTrigger tem um método para trancar/destrancar.
            // Se o seu script usa uma bool `isLocked`, teríamos que ajustar.
            // Por enquanto, vamos assumir que o trigger está desativado.
            finalTransitionTrigger.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // Se o jogador não foi definido, tenta encontrá-lo na cena.
        if (playerTransform == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("[AylaForestGuide] Jogador não encontrado! O tour não pode começar.");
                gameObject.SetActive(false);
                return;
            }
        }
        
        // Inicia o tour
        isTourActive = true;
    }
    

    void Update()
    {
        // Se o tour não está ativo ou não temos um caminho a seguir, não faz nada.
        if (!isTourActive || waypoints.Count == 0 || playerTransform == null)
        {
            SetAnimation(Vector2.zero); // Garante que ela fique parada
            return;
        }

        // Verifica se já chegamos ao final do caminho.
        if (currentWaypointIndex >= waypoints.Count)
        {
            HandleTourCompletion();
            return;
        }

        // --- LÓGICA DE ESPERA ---
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistanceToPlayer)
        {
            isWaitingForPlayer = true;
        }
        else if (distanceToPlayer < resumeDistanceToPlayer)
        {
            isWaitingForPlayer = false;
        }

        // Se estiver esperando, para a animação e interrompe o resto da lógica.
        if (isWaitingForPlayer)
        {
            SetAnimation(Vector2.zero);
            return;
        }

        // --- LÓGICA DE MOVIMENTO ---
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Move Ayla em direção ao waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        SetAnimation(direction);

        // Verifica se chegou ao waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // --- LÓGICA DE DIÁLOGO ---
            if (currentWaypointIndex == dialogueTriggerWaypointIndex && !dialogueTriggered)
            {
                TriggerDialogue();
            }
            
            currentWaypointIndex++; // Avança para o próximo waypoint
        }
    }

    private void TriggerDialogue()
{
    if (tourDialogue != null && DialogueManager.Instance != null)
    {
        Debug.Log("[AylaForestGuide] Acionando diálogo do tour em modo automático.");
        //                                                      vvv PASSE O NOVO PARÂMETRO vvv
        DialogueManager.Instance.StartDialogue(tourDialogue, true); 
        dialogueTriggered = true;
    }
}

    private void HandleTourCompletion()
{
    Debug.Log("[AylaForestGuide] Tour concluído! Ayla vai desaparecer.");
    isTourActive = false;
    SetAnimation(Vector2.zero);

    // 1. Marca o tour como concluído para não acontecer de novo.
    PlayerPrefs.SetInt(tourCompletionFlag, 1);
    PlayerPrefs.Save();

    // 2. Ativa a porta de transição para o jogador.
    if (finalTransitionTrigger != null)
    {
        finalTransitionTrigger.gameObject.SetActive(true);
    }

    // 3. Ayla desaparece.
    gameObject.SetActive(false);
}

    private void SetAnimation(Vector2 direction)
    {
        if (animator == null) return;

        // Assumindo que a animação de andar para os lados é o estado 5
        // e a de ficar parada de lado é o estado 4 (baseado no seu NPCTourGuide.cs)
        if (direction.magnitude > 0.1f)
        {
            animator.SetInteger("MovementState", 5); // Andando de lado
            // Vira o sprite para a direção do movimento
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            animator.SetInteger("MovementState", 4); // Parada de lado
        }
    }
}