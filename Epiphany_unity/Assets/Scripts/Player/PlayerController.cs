using UnityEngine;
using UnityEngine.InputSystem; // Necessário para o novo Input System
using Epiphany.Input;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions playerInputActions; // Referência ao asset de Input Actions

    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    public float _playerSpeed = 5f; // Boa prática: inicialize valores públicos no código
    private Vector2 _playerDirection;
    private SpriteRenderer _spriteRenderer;

    private bool _isFacingRight = true;
    private int _lastVerticalDirection = 0; // 0: idle/horizontal, 1: up, -1: down

    void Awake()
    {
        // Inicializa a classe gerada do Input System
        playerInputActions = new PlayerInputActions();

        // Assina as ações de botão para chamar métodos específicos
        // '.performed' significa que a ação foi completamente executada (tecla pressionada e solta, ou pressionada e mantida)
        playerInputActions.Gameplay.Interact.performed += OnInteractPerformed;
        playerInputActions.Gameplay.AdvanceDialogue.performed += OnAdvanceDialoguePerformed;
        playerInputActions.Gameplay.OpenMenu.performed += OnOpenMenuPerformed;
        playerInputActions.Gameplay.OpenMap.performed += OnOpenMapPerformed;

        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_playerRigidbody2D == null) Debug.LogError("Rigidbody2D not found on player!");
        if (_playerAnimator == null) Debug.LogWarning("Animator not found on player!");
        if (_spriteRenderer == null) Debug.LogWarning("SpriteRenderer not found on player!");
    }

    private void OnEnable()
    {
        playerInputActions.Gameplay.Enable(); // Habilita o Action Map 'Gameplay' quando o GameObject é ativado
    }

    private void OnDisable()
    {
        playerInputActions.Gameplay.Disable(); // Desabilita o Action Map 'Gameplay' quando o GameObject é desativado

        // MUITO IMPORTANTE: Desassinar os eventos para evitar vazamento de memória e erros
        playerInputActions.Gameplay.Interact.performed -= OnInteractPerformed;
        playerInputActions.Gameplay.AdvanceDialogue.performed -= OnAdvanceDialoguePerformed;
        playerInputActions.Gameplay.OpenMenu.performed -= OnOpenMenuPerformed;
        playerInputActions.Gameplay.OpenMap.performed -= OnOpenMapPerformed;
    }

    void Start()
    {
        Debug.Log($"Player ({gameObject.name}) Start() chamado.");
    }

    void Update()
    {
        // Lê o valor da ação 'Move' do Input System (um Vector2)
        _playerDirection = playerInputActions.Gameplay.Move.ReadValue<Vector2>();

        // Usamos as componentes X e Y do _playerDirection para a lógica de animação e flip
        float moveX = _playerDirection.x;
        float moveY = _playerDirection.y;

        int currentMovementState = 0; // 0: ParadoBaixo, 1: Correndo, 2: Cima, 3: Baixo, 4: ParadoCima

        if (moveY > 0.1f) // Movendo para Cima
        {
            currentMovementState = 2; // Andando Cima
            _lastVerticalDirection = 1;
        }
        else if (moveY < -0.1f) // Movendo para Baixo
        {
            currentMovementState = 3; // Andando Baixo
            _lastVerticalDirection = -1;
        }
        else if (Mathf.Abs(moveX) > 0.1f) // Movendo Horizontalmente
        {
            currentMovementState = 1; // Correndo
            // Não resetar _lastVerticalDirection aqui, para saber qual idle usar
        }
        else // Parado
        {
            if (_lastVerticalDirection == 1) // Última direção vertical foi para cima
            {
                currentMovementState = 4; // Parado Cima
            }
            else // Última direção vertical foi para baixo ou estava andando horizontalmente
            {
                currentMovementState = 0; // Parado Baixo (ou idle padrão)
            }
        }
        
        if (_playerAnimator != null)
        {
            _playerAnimator.SetInteger("MovementState", currentMovementState);
            // Também podemos passar as direções X e Y para o Animator se ele as usar para blend tree
            _playerAnimator.SetFloat("MoveX", moveX);
            _playerAnimator.SetFloat("MoveY", moveY);
        }
        Flip(moveX, currentMovementState);
    }

    void FixedUpdate()
    {
        if (_playerRigidbody2D != null)
        {
            _playerRigidbody2D.linearVelocity = _playerDirection * _playerSpeed; // Use .velocity para Rigidbody2D
        }
    }

    void Flip(float moveX, int currentMovementState)
    {
        // Só flipa se estiver no estado de corrida (ou qualquer estado horizontal)
        if (currentMovementState == 1) // Correndo
        {
            if (moveX > 0.01f && !_isFacingRight)
            {
                _isFacingRight = true;
                if (_spriteRenderer != null) _spriteRenderer.flipX = false;
            }
            else if (moveX < -0.01f && _isFacingRight)
            {
                _isFacingRight = false;
                if (_spriteRenderer != null) _spriteRenderer.flipX = true;
            }
        }
    }

    // Métodos de tratamento para as ações de botão (chamados pelo Input System)
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Ação de Interagir (E) executada!");
        // *** FUTURA LÓGICA DE INTERAÇÃO AQUI ***
        // Ex: Chamar um DialogueManager.Instance.TryInteractWithNPC();
    }

    private void OnAdvanceDialoguePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Ação de Avançar Diálogo/Menu (Enter/Espaço) executada!");
        // *** FUTURA LÓGICA DE AVANÇO DE DIÁLOGO AQUI ***
        // Ex: DialogueManager.Instance.AdvanceDialogue();
    }

    private void OnOpenMenuPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Ação de Abrir Menu (Esc) executada!");
        // *** FUTURA LÓGICA DE ABRIR MENU AQUI ***
        // Ex: UIManager.Instance.TogglePauseMenu();
    }

    private void OnOpenMapPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Ação de Abrir Mapa (F) executada!");
        // *** FUTURA LÓGICA DE ABRIR MAPA AQUI ***
        // Ex: UIManager.Instance.ToggleMap();
    }
}