using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

// Nova classe para definir um waypoint com eventos.
// [System.Serializable] permite que ela apareça no Inspector.
[System.Serializable]
public class WaypointEvent
{
    public Transform target;
    [Tooltip("Evento disparado quando o NPC chega a este waypoint.")]
    public UnityEvent onWaypointReached;
}

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class NPCTourGuide : MonoBehaviour
{
    [Header("Configuração do Tour")]
    [Tooltip("A lista de waypoints e eventos que o NPC seguirá.")]
    [SerializeField] private List<WaypointEvent> tourPath;
    [SerializeField] private float moveSpeed = 2.0f;

    [Header("Comportamento de Espera")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float maxDistanceToPlayer = 5.0f;
    [SerializeField] private float resumeDistanceToPlayer = 3.0f;

    [Header("Eventos Globais")]
    [Tooltip("Disparado quando o NPC chega ao último waypoint do tour.")]
    public UnityEvent OnTourCompleted;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int currentWaypointIndex = 0;
    private bool isWaitingForPlayer = false;
    private bool isTourActive = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enabled = false;
    }

    public void StartTour()
    {
        if (playerTransform == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null) playerTransform = player.transform;
            else
            {
                Debug.LogError("[NPCTourGuide] Jogador não encontrado. O tour não pode começar.");
                return;
            }
        }
        isTourActive = true;
        this.enabled = true;
    }

    void Update()
    {
        if (!isTourActive || tourPath.Count == 0 || playerTransform == null)
        {
            SetAnimation(Vector2.zero);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistanceToPlayer) isWaitingForPlayer = true;
        else if (distanceToPlayer < resumeDistanceToPlayer) isWaitingForPlayer = false;

        if (isWaitingForPlayer)
        {
            SetAnimation(Vector2.zero);
            return;
        }

        if (currentWaypointIndex >= tourPath.Count)
        {
            // Tour concluído
            isTourActive = false;
            SetAnimation(Vector2.zero);
            this.enabled = false;
            OnTourCompleted.Invoke();
            return;
        }

        Transform targetWaypoint = tourPath[currentWaypointIndex].target;
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        SetAnimation(direction);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Dispara o evento do waypoint que acabamos de alcançar.
            tourPath[currentWaypointIndex].onWaypointReached.Invoke();
            currentWaypointIndex++;
        }
    }

    private void SetAnimation(Vector2 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            animator.SetInteger("MovementState", 5);
            spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            animator.SetInteger("MovementState", 4);
        }
    }

    // Novo método público para ser chamado por um evento de waypoint.
    public void PauseTour()
    {
        isTourActive = false;
    }
}