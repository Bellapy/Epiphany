using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    public float _playerSpeed;
    private Vector2 _playerDirection;
    private SpriteRenderer _spriteRenderer;

    private bool _isFacingRight = true;
    private int _lastVerticalDirection = 0; // 0 = neutro/nenhum, 1 = cima, -1 = baixo

    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            // Mantém _lastVerticalDirection para saber para qual idle voltar se parar
            // ou reseta se preferir que o movimento lateral "cancele" a direção vertical para o idle
            // _lastVerticalDirection = 0; // Descomente se quiser que o movimento lateral resete a direção do idle
        }
        else // Parado
        {
            if (_lastVerticalDirection == 1) // Estava movendo para cima por último
            {
                currentMovementState = 4; // 4 = Parado de Costas
            }
            else if (_lastVerticalDirection == -1) // Estava movendo para baixo por último
            {
                // Se o seu estado 0 já é "parado de frente", use ele.
                // Se você criou um estado 5 para "parado de frente" específico, use-o.
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
        _playerRigidbody2D.MovePosition(_playerRigidbody2D.position + _playerDirection * _playerSpeed * Time.fixedDeltaTime);
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
        else
        {
            
        }
    }
}