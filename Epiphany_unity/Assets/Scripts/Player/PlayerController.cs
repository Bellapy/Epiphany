using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ... (suas variáveis de sempre) ...
    [Header("Configurações de Movimento")] [SerializeField] private float moveSpeed = 5f;
    [Header("Referências de Componentes")] private Rigidbody2D rb; private Animator animator; private SpriteRenderer spriteRenderer; private PlayerInput playerInput;
    [Header("Controle de Input e Estado")] private Vector2 currentMovementInput; private bool isFacingRight = true; private int lastVerticalDirection = -1; private bool canMove = true; private bool isInCutscene = false;

    // --- Variáveis para a Lógica da Escada ---
    private bool canClimb = false;
    private LadderZone currentLadderZone; // Referência para a zona de escada atual

    private RigidbodyType2D originalBodyType;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        if (rb != null) { originalBodyType = rb.bodyType; }
    }

    void FixedUpdate()
    {
        if (canMove && !isInCutscene) {
            rb.linearVelocity = currentMovementInput * moveSpeed;
        } else {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void Update()
    {
        if (isInCutscene) return;

        // <<< A NOVA LÓGICA DE ATIVAÇÃO ESTÁ AQUI >>>
        // Se o jogador pode subir E apertou para cima...
        if (canClimb && currentMovementInput.y > 0.1f && currentLadderZone != null)
        {
            // ...inicia a sequência automática!
            canClimb = false; // Impede que seja chamado de novo
            currentLadderZone.StartAutomaticClimb(this);
            return; // Para a execução do Update para evitar conflitos de animação
        }

        UpdateAnimationsAndFlip();
    }

    public void OnMove(InputValue value)
    {
        if (canMove) { currentMovementInput = value.Get<Vector2>(); }
    }
    
    public void EnableMovement()
    {
        Debug.Log("Habilitando movimento do jogador.");
        canMove = true;
        isInCutscene = false;
        if(rb != null) { rb.bodyType = originalBodyType; }
        if (playerInput != null) { playerInput.SwitchCurrentActionMap("Player"); }
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

   
    if (animator != null)
    {
        // Se a última direção foi para cima, usa o idle de costas (estado 4), senão, o de frente (estado 0).
        int idleState = (lastVerticalDirection == 1) ? 4 : 0;
        animator.SetInteger("MovementState", idleState);
    }
    
    if (playerInput != null)
    {
        playerInput.SwitchCurrentActionMap("PuzzleUI");
    }
}

    private void UpdateAnimationsAndFlip()
    {
        if (animator == null || !canMove) return;

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
    
    // Agora este método recebe a referência da LadderZone
    public void SetCanClimb(bool status, LadderZone ladder)
    {
        canClimb = status;
        currentLadderZone = ladder;
    }

    public void StartClimbingSequence(Transform startPoint, Transform endPoint, string sceneName, string spawnName)
    {
        StartCoroutine(ClimbStairsCoroutine(startPoint, endPoint, sceneName, spawnName));
    }

    private IEnumerator ClimbStairsCoroutine(Transform startPoint, Transform endPoint, string sceneName, string spawnName)
    {
        isInCutscene = true;
        DisableMovement();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        while (Vector3.Distance(transform.position, startPoint.position) > 0.05f) {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPoint.position;
        animator.SetInteger("MovementState", 2);

        float climbDuration = 2.0f;
        float timer = 0f;
        Vector3 initialPos = transform.position;

        while (timer < climbDuration) {
            transform.position = Vector3.Lerp(initialPos, endPoint.position, timer / climbDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        if (FadeController.Instance != null) {
            FadeController.Instance.StartFadeOut(() => {
                GameManager.Instance.SetNextSpawnPoint(spawnName);
                GameManager.Instance.LoadScene(sceneName);
            });
        } else {
            GameManager.Instance.SetNextSpawnPoint(spawnName);
            GameManager.Instance.LoadScene(sceneName);
        }
    }
}