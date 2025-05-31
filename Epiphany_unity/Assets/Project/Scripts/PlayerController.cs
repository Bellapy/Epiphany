using UnityEngine;
using UnityEngine.SceneManagement; // <-- ADICIONADO: Necessário para SceneManager e PlayerPrefs

public class NewMonoBehaviourScript : MonoBehaviour
{
    // --- NOVO: Para garantir que só exista uma personagem por cena (evita duplicatas se você voltar para uma cena) ---
    public static NewMonoBehaviourScript Instance; 

    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    public float _playerSpeed;
    private Vector2 _playerDirection;
    private SpriteRenderer _spriteRenderer;

    private bool _isFacingRight = true;
    private int _lastVerticalDirection = 0; // 0 = neutro/nenhum, 1 = cima, -1 = baixo

    // --- NOVO MÉTODO: Chamado antes de Start() ---
    void Awake()
    {
        // Lógica de Singleton para garantir que haja apenas uma instância do player
        if (Instance == null)
        {
            Instance = this; // Define esta instância como a única
            DontDestroyOnLoad(gameObject); // Faz com que o GameObject da personagem não seja destruído ao carregar novas cenas
        }
        else // Se já existe uma instância (ex: ao voltar para uma cena que já tem um player), destrua esta nova.
        {
            Destroy(gameObject); 
            return; // Sai do método para evitar que o código de Start() seja executado nesta duplicata.
        }

        // É uma boa prática mover os GetComponent para Awake() quando se usa DontDestroyOnLoad
        // para garantir que as referências estejam prontas antes de qualquer Start() ou Update()
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Boa prática: Verificar se os componentes foram encontrados (para depuração)
        if (_playerRigidbody2D == null) Debug.LogError("Rigidbody2D not found on player!");
        if (_playerAnimator == null) Debug.LogWarning("Animator not found on player!");
        if (_spriteRenderer == null) Debug.LogWarning("SpriteRenderer not found on player!");
    }

    void Start()
    {
        // Se você moveu os GetComponents para Awake(), pode removê-los daqui
        // if (_playerRigidbody2D == null) _playerRigidbody2D = GetComponent<Rigidbody2D>();
        // if (_playerAnimator == null) _playerAnimator = GetComponent<Animator>();
        // if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

        // --- NOVO: Chama a lógica para posicionar a personagem no ponto de spawn na nova cena ---
        SetSpawnPosition();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        _playerDirection = new Vector2(moveX, moveY).normalized;

        int currentMovementState = 0; // Padrão para parado (frente/lado)

        if (moveY > 0.1f) // Movendo para Cima
        {
            currentMovementState = 2; // 2 = Andando para Cima (Costas)
            _lastVerticalDirection = 1;
        }
        else if (moveY < -0.1f) // Movendo para Baixo
        {
            currentMovementState = 3; // 3 = Andando para Baixo (Frente)
            _lastVerticalDirection = -1;
        }
        else if (Mathf.Abs(moveX) > 0.1f) // Movendo para os Lados
        {
            currentMovementState = 1; // 1 = Andando para o Lado
        }
        else // Parado
        {
            if (_lastVerticalDirection == 1) // Estava movendo para cima por último
            {
                currentMovementState = 4; // 4 = Parado de Costas
            }
            else if (_lastVerticalDirection == -1) // Estava movendo para baixo por último
            {
                currentMovementState = 0; // Ou 5, se você criou "ParadoDeFrente"
            }
            else // Parado e não houve movimento vertical recente (ou foi resetado)
            {
                currentMovementState = 0; // Parado padrão (frente/lado)
            }
        }

        _playerAnimator.SetInteger("Movimento", currentMovementState);
        Flip(moveX, currentMovementState); // Passar o estado atual para o Flip
    }

    void FixedUpdate()
    {
        // --- CORREÇÃO FINAL: Use 'velocity' com 'v' minúsculo! ---
        _playerRigidbody2D.linearVelocity = _playerDirection * _playerSpeed;
    }

    void Flip(float moveX, int currentMovementState)
    {
        // Só flipa se estiver no estado de andar de lado
        if (currentMovementState == 1) // 1 é Andando para o Lado
        {
            if (moveX > 0.01f && !_isFacingRight)
            {
                _isFacingRight = true;
                _spriteRenderer.flipX = false;
            }
            else if (moveX < -0.01f && _isFacingRight)
            {
                _isFacingRight = false;
                _spriteRenderer.flipX = true;
            }
        }
        // else { } // Pode remover este 'else' vazio se não faz nada
    }

    // --- NOVO MÉTODO: Para posicionar a personagem ao carregar uma nova cena ---
    void SetSpawnPosition()
    {
        // Obtém o nome do ponto de spawn da PlayerPrefs que a porta salvou
        string lastEnteredDoorName = PlayerPrefs.GetString("LastEnteredDoor", "");

        if (!string.IsNullOrEmpty(lastEnteredDoorName))
        {
            // Tenta encontrar o GameObject do ponto de spawn na cena atual
            GameObject spawnPoint = GameObject.Find(lastEnteredDoorName);
            if (spawnPoint != null)
            {
                // Posiciona o Rigidbody da personagem no local do ponto de spawn
                _playerRigidbody2D.position = spawnPoint.transform.position;
                Debug.Log($"Personagem posicionada em {spawnPoint.name} ({spawnPoint.transform.position})");
            }
            else
            {
                Debug.LogWarning($"Ponto de spawn '{lastEnteredDoorName}' não encontrado na cena '{SceneManager.GetActiveScene().name}'. Verifique o nome no Inspector da porta e na cena de destino.");
            }
            // Limpa o PlayerPrefs para evitar spawn em local errado em cargas futuras
            PlayerPrefs.DeleteKey("LastEnteredDoor");
            PlayerPrefs.Save(); // Salva as alterações no PlayerPrefs
        }
    }
}