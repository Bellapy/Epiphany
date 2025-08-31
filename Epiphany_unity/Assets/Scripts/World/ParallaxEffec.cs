using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Alvo do Parallax")]
    [Tooltip("O Transform que o efeito de parallax vai seguir. Para câmera estática, use o Player.")]
    [SerializeField] private Transform targetToFollow;

    [Header("Configuração do Efeito")]
    [Tooltip("Força do efeito. Positivo para fundos, negativo para primeiros planos.")]
    [SerializeField] private float parallaxMultiplierX;

    // Variáveis internas para o cálculo
    private Vector3 targetStartPosition;
    private Vector3 selfStartPosition;

    void Start()
    {
        if (targetToFollow == null)
        {
            Debug.LogError("Alvo para o Parallax não foi definido!", this.gameObject);
            this.enabled = false; // Desativa o script se não houver alvo
            return;
        }
        
        // Guarda as posições iniciais do alvo e desta camada de parallax
        targetStartPosition = targetToFollow.position;
        selfStartPosition = transform.position;
    }

    void LateUpdate()
    {
        // Calcula o quanto o alvo se moveu da sua posição inicial, apenas no eixo X
        float deltaX = targetToFollow.position.x - targetStartPosition.x;

        // Calcula a nova posição para esta camada de parallax
        // O movimento é na direção OPOSTA ao do alvo, por isso o sinal negativo
        float newPositionX = selfStartPosition.x - (deltaX * parallaxMultiplierX);

        // Aplica a nova posição, mantendo o Y e Z originais
        transform.position = new Vector3(newPositionX, selfStartPosition.y, selfStartPosition.z);
    }
}