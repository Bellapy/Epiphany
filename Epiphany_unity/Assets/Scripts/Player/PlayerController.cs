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

    [Header("Controle de Input e Estado")]
    private Vector2 currentMovementInput;
    private bool isFacingRight = true;
    private int lastVerticalDirection = -1; // -1 para baixo, 1 para cima

    // --- Ciclo de Vida: Awake() ---
    void Awake()
    {
        // Apenas pega as referências dos componentes.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null) Debug.LogError("Rigidbody2D não encontrado no jogador!");
        if (animator == null) Debug.LogWarning("Animator não encontrado no jogador!");
        if (spriteRenderer == null) Debug.LogWarning("SpriteRenderer não encontrado no jogador!");
    }

    // --- Ciclo de Vida: FixedUpdate() ---
    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = currentMovementInput * moveSpeed;
        }
    }
    
    // --- Ciclo de Vida: Update() ---
    void Update()
    {
        UpdateAnimationsAndFlip();
    }

    // --- Métodos de Tratamento para as Ações de Input ---
    // O componente Player Input na Unity vai chamar esses métodos automaticamente.

    // Este método é chamado quando a ação "Move" é ativada.
    public void OnMove(InputValue value)
    {
        currentMovementInput = value.Get<Vector2>();
    }
    
    // (Você pode adicionar os outros métodos OnInteract, OnOpenMenu, etc., aqui)

    // --- Lógica de Animação e Visual ---
    private void UpdateAnimationsAndFlip()
    {
        if (animator == null) return;

        float moveX = currentMovementInput.x;
        float moveY = currentMovementInput.y;

        int currentMovementState = 0; // Estado padrão: Parado Baixo

        if (moveY > 0.1f)
        {
            currentMovementState = 2; // Estado: Andando Cima
            lastVerticalDirection = 1;
        }
        else if (moveY < -0.1f)
        {
            currentMovementState = 3; // Estado: Andando Baixo
            lastVerticalDirection = -1;
        }
        else if (Mathf.Abs(moveX) > 0.1f)
        {
            currentMovementState = 1; // Estado: Correndo Lado
        }
        else
        {
            if (lastVerticalDirection == 1)
            {
                currentMovementState = 4; // Estado: Parado Cima
            }
            else
            {
                currentMovementState = 0; // Estado: Parado Baixo
            }
        }
        
        animator.SetInteger("MovementState", currentMovementState);

        // Lógica de Flip
        if (spriteRenderer != null && currentMovementState == 1)
        {
            if (moveX > 0 && !isFacingRight)
            {
                isFacingRight = true;
                spriteRenderer.flipX = false;
            }
            else if (moveX < 0 && isFacingRight)
            {
                isFacingRight = false;
                spriteRenderer.flipX = true;
            }
        }
    }
}