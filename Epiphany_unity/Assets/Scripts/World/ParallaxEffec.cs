using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Alvo do Parallax")]
    [Tooltip("Arraste o Transform do seu Player aqui.")]
    [SerializeField] private Transform targetToFollow;

    [Header("Configuração do Efeito")]
    [Tooltip("A força do efeito. Positivo para fundos, negativo para primeiros planos.")]
    [SerializeField] private float parallaxMultiplierX;

    // Variáveis internas
    private Vector3 targetStartPosition;
    private Vector3 selfStartPosition;

    void Start()
    {
        if (targetToFollow == null)
        {
            Debug.LogError("Alvo para o Parallax não foi definido! (Arraste a Player)", this.gameObject);
            this.enabled = false;
            return;
        }
        
        targetStartPosition = targetToFollow.position;
        selfStartPosition = transform.position;
    }

    void LateUpdate()
    {
        float deltaX = targetToFollow.position.x - targetStartPosition.x;

        float newPositionX = selfStartPosition.x - (deltaX * parallaxMultiplierX);


        transform.position = new Vector3(newPositionX, selfStartPosition.y, selfStartPosition.z);
    }
}