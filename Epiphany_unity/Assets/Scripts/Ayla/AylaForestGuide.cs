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

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int currentWaypointIndex = 0;
    private bool isWaitingForPlayer = false;
    private bool isTourActive = false;
    private bool dialogueTriggered = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (PlayerPrefs.GetInt(tourCompletionFlag, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (finalTransitionTrigger != null)
        {
            finalTransitionTrigger.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (playerTransform == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }
        }
        
        isTourActive = true;
    }
    
    void Update()
    {
        if (!isTourActive || waypoints.Count == 0 || playerTransform == null)
        {
            SetAnimation(Vector2.zero);
            return;
        }

        if (currentWaypointIndex >= waypoints.Count)
        {
            HandleTourCompletion();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistanceToPlayer)
        {
            isWaitingForPlayer = true;
        }
        else if (distanceToPlayer < resumeDistanceToPlayer)
        {
            isWaitingForPlayer = false;
        }

        if (isWaitingForPlayer)
        {
            SetAnimation(Vector2.zero);
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        SetAnimation(direction);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            if (currentWaypointIndex == dialogueTriggerWaypointIndex && !dialogueTriggered)
            {
                TriggerDialogue();
            }
            
            currentWaypointIndex++;
        }
    }

    private void TriggerDialogue()
    {
        if (tourDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(tourDialogue, true); 
            dialogueTriggered = true;
        }
    }

    private void HandleTourCompletion()
    {
        isTourActive = false;
        SetAnimation(Vector2.zero);

        PlayerPrefs.SetInt(tourCompletionFlag, 1);
        PlayerPrefs.Save();

        if (finalTransitionTrigger != null)
        {
            finalTransitionTrigger.gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    private void SetAnimation(Vector2 direction)
    {
        if (animator == null) return;

        if (direction.magnitude > 0.1f)
        {
            animator.SetInteger("MovementState", 5);
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            animator.SetInteger("MovementState", 4);
        }
    }
}