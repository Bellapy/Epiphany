using UnityEngine;

public class AylaFollowController : MonoBehaviour
{
    [Header("Alvo a Seguir")]
    [SerializeField] private Transform playerTransform;

    [Header("Comportamento de Seguir")]
    [SerializeField] private float distanciaAlvo = 2.0f;

    [Header("Referências da Ayla")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;

    // As variáveis de velocidade e memória da animação são gerenciadas internamente agora.
    private float velocidadeMovimento = 5.5f; // Um pouco mais rápida que a player
    private int aylaLastHorizontalDirection = 1;

    void Awake()
    {
        if (aylaAnimator == null) aylaAnimator = GetComponent<Animator>();
        if (aylaSpriteRenderer == null) aylaSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            AtualizarAnimacaoAyla(Vector2.zero);
            return;
        }

        float distanciaX = playerTransform.position.x - transform.position.x;
        Vector2 direcaoMovimento = Vector2.zero;

        // Ayla SÓ se move se a distância horizontal for MAIOR que a distância alvo.
        if (Mathf.Abs(distanciaX) > distanciaAlvo)
        {
            float direcaoX = Mathf.Sign(distanciaX);
            direcaoMovimento = new Vector2(direcaoX, 0);
        }
        
        AtualizarAnimacaoAyla(direcaoMovimento);
        transform.position += (Vector3)direcaoMovimento * velocidadeMovimento * Time.deltaTime;
    }
    
    private void AtualizarAnimacaoAyla(Vector2 direcaoMovimento)
    {
        if (aylaAnimator == null) return;

        int estadoMovimento;
        bool estaMovendo = Mathf.Abs(direcaoMovimento.x) > 0.1f;

        if (estaMovendo)
        {
            estadoMovimento = 5; // Andando de Lado
            aylaLastHorizontalDirection = (direcaoMovimento.x > 0) ? 1 : -1;
        }
        else
        {
            estadoMovimento = 4; // Parada de Lado
        }
        
        aylaAnimator.SetInteger("MovementState", estadoMovimento);
        
        if (aylaLastHorizontalDirection != 0)
        {
             aylaSpriteRenderer.flipX = aylaLastHorizontalDirection == -1;
        }
    }
}