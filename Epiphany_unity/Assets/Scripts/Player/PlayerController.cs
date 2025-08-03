using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Necessário para a Corrotina

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
    private bool canMove = true; 
    
    // <<< NOVA FLAG DE CONTROLE >>>
    private bool isInCutscene = false;

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
        // A lógica de movimento agora também verifica se não estamos em uma cutscene.
        if (canMove && !isInCutscene)
        {
            rb.linearVelocity = currentMovementInput * moveSpeed;
        }
    }
    
    void Update()
    {
        // <<< PEQUENO AJUSTE AQUI >>>
        // Se estivermos em uma cutscene, a lógica de animação do Update é ignorada.
        if (isInCutscene) return;

        UpdateAnimationsAndFlip();
    }

    public void OnMove(InputValue value)
    {
        if (canMove)
        {
            currentMovementInput = value.Get<Vector2>();
        }
    }
    
    public void EnableMovement()
    {
        Debug.Log("Habilitando movimento do jogador.");
        canMove = true;
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void DisableMovement()
    {
        Debug.Log("Desabilitando movimento do jogador.");
        canMove = false;
        currentMovementInput = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("PuzzleUI"); // Ou um mapa "UI" genérico
        }
    }

    // --- LÓGICA DE ANIMAÇÃO E VISUAL ---
    private void UpdateAnimationsAndFlip()
    {
        if (animator == null) return;
        if (!canMove)
        {
            animator.SetInteger("MovementState", 0);
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
    
    // <<< NOVO MÉTODO E CORROTINA PARA A ESCADA >>>

    /// <summary>
    /// Método público chamado pelo StairTrigger para iniciar a sequência.
    /// </summary>
    public void StartClimbingSequence(Transform startPoint, Transform endPoint, string sceneName, string spawnName)
    {
        StartCoroutine(ClimbStairsCoroutine(startPoint, endPoint, sceneName, spawnName));
    }

    private IEnumerator ClimbStairsCoroutine(Transform startPoint, Transform endPoint, string sceneName, string spawnName)
    {
        // 1. Tomar controle total do jogador
        isInCutscene = true;
        DisableMovement();
        rb.isKinematic = true; // Desliga a física temporariamente
        rb.linearVelocity = Vector2.zero;

        // 2. Mover suavemente para o ponto de partida da escada
        while (Vector3.Distance(transform.position, startPoint.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPoint.position;

        // 3. Tocar a animação de subir escada (usando a de andar para cima, estado 2)
        animator.SetInteger("MovementState", 2);

        // 4. Mover para cima
        float climbDuration = 2.0f; // Duração da subida em segundos
        float timer = 0f;
        Vector3 initialPos = transform.position;

        while (timer < climbDuration)
        {
            transform.position = Vector3.Lerp(initialPos, endPoint.position, timer / climbDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // 5. Iniciar o fade out e trocar de cena
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeOut(() => {
                // Este código só roda quando a tela está totalmente preta
                GameManager.Instance.SetNextSpawnPoint(spawnName);
                GameManager.Instance.LoadScene(sceneName);
            });
        }
        else // Plano B: Se não houver FadeController, troca de cena direto
        {
            GameManager.Instance.SetNextSpawnPoint(spawnName);
            GameManager.Instance.LoadScene(sceneName);
        }
    }
}