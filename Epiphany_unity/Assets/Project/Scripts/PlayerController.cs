using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public static NewMonoBehaviourScript Instance;

    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    public float _playerSpeed;
    private Vector2 _playerDirection;
    private SpriteRenderer _spriteRenderer;

    private bool _isFacingRight = true;
    private int _lastVerticalDirection = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_playerRigidbody2D == null) Debug.LogError("Rigidbody2D not found on player!");
        if (_playerAnimator == null) Debug.LogWarning("Animator not found on player!");
        if (_spriteRenderer == null) Debug.LogWarning("SpriteRenderer not found on player!");
    }

    void Start()
    {
        SetSpawnPosition();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        _playerDirection = new Vector2(moveX, moveY).normalized;

        int currentMovementState = 0;

        if (moveY > 0.1f)
        {
            currentMovementState = 2;
            _lastVerticalDirection = 1;
        }
        else if (moveY < -0.1f)
        {
            currentMovementState = 3;
            _lastVerticalDirection = -1;
        }
        else if (Mathf.Abs(moveX) > 0.1f)
        {
            currentMovementState = 1;
        }
        else
        {
            if (_lastVerticalDirection == 1)
            {
                currentMovementState = 4;
            }
            else if (_lastVerticalDirection == -1)
            {
                currentMovementState = 0;
            }
            else
            {
                currentMovementState = 0;
            }
        }

        _playerAnimator.SetInteger("Movimento", currentMovementState);
        Flip(moveX, currentMovementState);
    }

    void FixedUpdate()
    {
        _playerRigidbody2D.linearVelocity = _playerDirection * _playerSpeed;  // CORRETO: velocity, não linearVelocity
    }

    void Flip(float moveX, int currentMovementState)
    {
        if (currentMovementState == 1)
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
    }

    // ========================== SPAWN ==========================
    
    // Método para definir a posição do player na cena com base no PlayerPrefs
    void SetSpawnPosition()
    {
        string spawnName = PlayerPrefs.GetString("NextSpawnPoint", null);

        if (string.IsNullOrEmpty(spawnName))
            return;

        GameObject spawnPoint = GameObject.Find(spawnName);
        if (spawnPoint != null)
        {
            _playerRigidbody2D.position = spawnPoint.transform.position;
            Debug.Log($"Player posicionado em: {spawnPoint.name}");
        }
        else
        {
            Debug.LogWarning($"Spawn '{spawnName}' não encontrado na cena.");
        }

        PlayerPrefs.DeleteKey("NextSpawnPoint");  // Apaga para não reaparecer sempre aqui
    }

    // Método estático para definir o spawn antes de trocar de cena
    public static void SetNextSpawnPoint(string spawnPointName)
    {
        PlayerPrefs.SetString("NextSpawnPoint", spawnPointName);
        PlayerPrefs.Save();  // salva imediatamente no disco
        Debug.Log($"Próximo ponto de spawn definido: {spawnPointName}");
    }
}
