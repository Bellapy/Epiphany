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
    private PlayerInput playerInput; // <-- NOVO: Referência ao componente PlayerInput

    [Header("Controle de Input e Estado")]
    private Vector2 currentMovementInput;
    private bool isFacingRight = true;
    private int lastVerticalDirection = -1;
    public bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>(); // <-- NOVO: Pega a referência

        if (rb == null) Debug.LogError("Rigidbody2D não encontrado no jogador!");
        if (animator == null) Debug.LogWarning("Animator não encontrado no jogador!");
        if (spriteRenderer == null) Debug.LogWarning("SpriteRenderer não encontrado no jogador!");
        if (playerInput == null) Debug.LogError("PlayerInput não encontrado no jogador!");
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (rb != null)
            {
                rb.linearVelocity = currentMovementInput * moveSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void Update()
    {
        UpdateAnimationsAndFlip();
    }

    public void OnMove(InputValue value)
    {
        if (canMove)
        {
            currentMovementInput = value.Get<Vector2>();
        }
        else
        {
            currentMovementInput = Vector2.zero;
        }
    }
    
    // --- Funções Públicas para Controle Externo ---
    
    public void EnableMovement()
    {
        canMove = true;
        // ATUALIZADO: Volta para o mapa de controle "Player"
        if (playerInput != null) playerInput.SwitchCurrentActionMap("Player");
    }

    public void DisableMovement()
    {
        canMove = false;
        // ATUALIZADO: Troca para o mapa de controle "PuzzleUI"
        if (playerInput != null) playerInput.SwitchCurrentActionMap("PuzzleUI");
    }

    // --- Lógica de Animação e Visual ---
    private void UpdateAnimationsAndFlip()
    {
        // ... (o resto desta função continua exatamente igual) ...
        if (animator == null) return;
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