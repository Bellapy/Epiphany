// NewMonoBehaviourScript.cs (Seu PlayerController)
using UnityEngine;
// Remova: using UnityEngine.SceneManagement; // Não é mais necessário aqui para o spawn

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Remova o padrão Singleton daqui
    // public static NewMonoBehaviourScript Instance;

    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    public float _playerSpeed;
    private Vector2 _playerDirection;
    private SpriteRenderer _spriteRenderer;

    private bool _isFacingRight = true;
    private int _lastVerticalDirection = 0; // 0: idle/horizontal, 1: up, -1: down

    void Awake()
    {
        // Remova a lógica do Singleton DontDestroyOnLoad
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_playerRigidbody2D == null) Debug.LogError("Rigidbody2D not found on player!");
        if (_playerAnimator == null) Debug.LogWarning("Animator not found on player!"); // Pode ser opcional dependendo do seu setup
        if (_spriteRenderer == null) Debug.LogWarning("SpriteRenderer not found on player!"); // Pode ser opcional
    }

    void Start()
    {
        // Remova a chamada SetSpawnPosition daqui. O GameManager cuidará disso.
        // SetSpawnPosition();
        // Você pode querer definir um estado inicial aqui se necessário,
        // mas o posicionamento será tratado pelo GameManager.
        Debug.Log($"Player ({gameObject.name}) Start() chamado.");
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        _playerDirection = new Vector2(moveX, moveY).normalized;

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
        
        // Apenas atualiza o _lastVerticalDirection se houver movimento vertical,
        // para que o idle correto seja mantido após movimento horizontal.
        // Se não houver movimento vertical (moveY está próximo de zero),
        // não altere _lastVerticalDirection, para que o idle correto seja mantido
        // ao parar de andar horizontalmente.

        if (_playerAnimator != null)
        {
            _playerAnimator.SetInteger("Movimento", currentMovementState);
        }
        Flip(moveX, currentMovementState);
    }

    void FixedUpdate()
    {
        if (_playerRigidbody2D != null)
        {
            // Mude _playerRigidbody2D.linearVelocity para _playerRigidbody2D.velocity
            _playerRigidbody2D.linearVelocity = _playerDirection * _playerSpeed;
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

    // Remova toda a seção de SPAWN daqui
    // ========================== SPAWN ==========================
    // void SetSpawnPosition() { ... }
    // public static void SetNextSpawnPoint(string spawnPointName) { ... }
}