using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Referências de Componentes")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;

    [Header("Controle de Input e Estado")]
    private Vector2 currentMovementInput;
    private bool isFacingRight = true;
    private int lastVerticalDirection = -1;
    // Removido o 'public' de 'canMove'. A melhor prática é controlar o estado
    // através de funções públicas, como Enable/DisableMovement.
    private bool canMove = true; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();

        if (rb == null) Debug.LogError("Rigidbody2D não encontrado no jogador!");
        if (animator == null) Debug.LogWarning("Animator não encontrado no jogador!");
        if (spriteRenderer == null) Debug.LogWarning("SpriteRenderer não encontrado no jogador!");
        if (playerInput == null) Debug.LogError("PlayerInput não encontrado no jogador! Este componente é essencial.");
    }

    void FixedUpdate()
    {
        // A lógica agora é mais simples: se puder mover, mova.
        if (canMove)
        {
            rb.linearVelocity = currentMovementInput * moveSpeed;
        }
    }
    
    void Update()
    {
        UpdateAnimationsAndFlip();
    }

    // Esta função é chamada pelo componente PlayerInput
    public void OnMove(InputValue value)
    {
        // A lógica de 'canMove' é aplicada aqui, no recebimento do input.
        if (canMove)
        {
            currentMovementInput = value.Get<Vector2>();
        }
    }
    
    // --- Funções Públicas para Controle Externo ---
    
    /// <summary>
    /// Permite que o jogador se mova e ativa o mapa de controle "Player".
    /// Chamado quando o puzzle termina.
    /// </summary>
    public void EnableMovement()
    {
        Debug.Log("Habilitando movimento do jogador.");
        canMove = true;
        if (playerInput != null)
        {
            // Garante que estamos no mapa de controle correto para andar.
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    /// <summary>
    /// Impede o jogador de se mover e muda para o mapa de controle "PuzzleUI".
    /// Chamado quando o puzzle começa.
    /// </summary>
    public void DisableMovement()
    {
        Debug.Log("Desabilitando movimento do jogador.");
        canMove = false;
        
        // Zera o movimento imediatamente para que a Ayla pare de deslizar.
        currentMovementInput = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (playerInput != null)
        {
            // Muda para o mapa de controle dos cristais.
            // O PlayerInput irá ignorar as ações de "Move" e ouvir apenas as do puzzle.
            playerInput.SwitchCurrentActionMap("PuzzleUI");
        }
    }

    // --- Lógica de Animação e Visual ---
    private void UpdateAnimationsAndFlip()
    {
        if (animator == null) return;
        
        // Se o movimento estiver desabilitado, garante que a animação fique parada.
        if (!canMove)
        {
            animator.SetInteger("MovementState", 0); // ou o estado de parado apropriado
            return;
        }

        float moveX = currentMovementInput.x;
        float moveY = currentMovementInput.y;
        int currentMovementState = 0;
        if (moveY > 0.1f) { currentMovementState = 2; lastVerticalDirection = 1; }
        else if (moveY < -0.1f) { currentMovementState = 3; lastVerticalDirection = -1; }
        else if (Mathf.Abs(moveX) > 0.1f) { currentMovementState = 1; }
        else { if (lastVerticalDirection == 1) { currentMovementState = 4; } else { currentMovementState = 0; } }
        animator.SetInteger("MovementState", currentMovementState);
        if (spriteRenderer != null && currentMovementState == 1) { if (moveX > 0 && !isFacingRight) { isFacingRight = true; spriteRenderer.flipX = false; } else if (moveX < 0 && isFacingRight) { isFacingRight = false; spriteRenderer.flipX = true; } }
    }
}