using UnityEngine;

public class AylaFollowController : MonoBehaviour
{
    [Header("Alvo a Seguir")]
    [Tooltip("Arraste o Transform da sua Player aqui.")]
    [SerializeField] private Transform playerTransform;

    [Header("Comportamento de Seguir")]
    [Tooltip("A que distância da player a Ayla deve tentar ficar.")]
    [SerializeField] private float distanciaAlvo = 2.0f;
    
    [Tooltip("A que distância mínima a Ayla para, para não ficar colada na player.")]
    [SerializeField] private float distanciaMinima = 1.5f;

    [Tooltip("A velocidade com que a Ayla anda.")]
    [SerializeField] private float velocidadeMovimento = 3.0f;

    [Header("Referências da Ayla")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;

    // "Memória" da Ayla para a animação de parada
    private int aylaLastVerticalDirection = -1;
    private int aylaLastHorizontalDirection = 0;

    void Awake()
    {
        // Pega as referências automaticamente se não forem arrastadas
        if (aylaAnimator == null) aylaAnimator = GetComponent<Animator>();
        if (aylaSpriteRenderer == null) aylaSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // DENTRO DE AylaFollowController.cs

void Update()
{
    if (playerTransform == null)
    {
        AtualizarAnimacaoAyla(Vector2.zero);
        return;
    }

    // --- LÓGICA DE MOVIMENTO HORIZONTAL ---

    // 1. Calcula a distância APENAS no eixo X.
    float distanciaX = playerTransform.position.x - transform.position.x;

    // A direção do movimento será -1 (esquerda), 1 (direita), ou 0 (parado).
    float direcaoX = 0;

    // 2. LÓGICA DE DECISÃO (agora focada no X)

    // Se a Ayla está muito "atrás" da player (à esquerda, e a player foi para a direita)
    if (distanciaX > distanciaAlvo)
    {
        direcaoX = 1; // Mover para a direita
    }
    // Se a Ayla está muito "à frente" da player (à direita, e a player foi para a esquerda)
    else if (distanciaX < -distanciaAlvo)
    {
        direcaoX = -1; // Mover para a esquerda
    }
    // Se a Ayla está muito perto da player...
    else if (Mathf.Abs(distanciaX) < distanciaMinima)
    {
        // Se a player está à direita, Ayla se afasta para a esquerda
        if (distanciaX > 0) direcaoX = -1;
        // Se a player está à esquerda, Ayla se afasta para a direita
        else direcaoX = 1;
    }
    
    // 3. APLICA O MOVIMENTO E A ANIMAÇÃO

    // Cria o vetor de movimento final, com Y sempre em zero.
    Vector2 direcaoMovimento = new Vector2(direcaoX, 0);

    // Atualiza a animação com base na direção calculada.
    AtualizarAnimacaoAyla(direcaoMovimento.normalized);

    // Move o transform da Ayla.
    transform.position += (Vector3)direcaoMovimento * velocidadeMovimento * Time.deltaTime;
}
    // O mesmo método de animação que já usamos antes.
    private void AtualizarAnimacaoAyla(Vector2 direcaoMovimento)
    {
        if (aylaAnimator == null) return;

        int estadoMovimento;
        bool estaMovendo = direcaoMovimento.magnitude > 0.1f;

        if (estaMovendo)
        {
            float moveX = direcaoMovimento.x;
            float moveY = direcaoMovimento.y;
            if (Mathf.Abs(moveY) > Mathf.Abs(moveX)) {
                if (moveY > 0) { estadoMovimento = 3; aylaLastVerticalDirection = 1; aylaLastHorizontalDirection = 0; } 
                else { estadoMovimento = 1; aylaLastVerticalDirection = -1; aylaLastHorizontalDirection = 0; }
            } else {
                estadoMovimento = 5; 
                aylaLastHorizontalDirection = (moveX > 0) ? 1 : -1;
                aylaLastVerticalDirection = 0;
            }
        }
        else
        {
            if (aylaLastHorizontalDirection != 0) {
                estadoMovimento = 4; 
            } else if (aylaLastVerticalDirection == 1) {
                estadoMovimento = 2; 
            } else {
                estadoMovimento = 0; 
            }
        }
        
        aylaAnimator.SetInteger("MovementState", estadoMovimento);
        
        if (estadoMovimento == 5 || estadoMovimento == 4) {
             aylaSpriteRenderer.flipX = direcaoMovimento.x < 0; // Usamos a direção atual para o flip
        } else {
            aylaSpriteRenderer.flipX = false;
        }
    }
}