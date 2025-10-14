using UnityEngine;
using System.Collections; // Necessário para usar Corrotinas

public class ParallaxEffect : MonoBehaviour
{
    [Header("Alvo do Parallax")]
    [Tooltip("Arraste o Transform do seu Player aqui.")]
    [SerializeField] private Transform targetToFollow;

    [Header("Configuração do Efeito")]
    [Tooltip("A força do efeito. Positivo para fundos, negativo para primeiros planos.")]
    [SerializeField] private float parallaxMultiplierX;

    private Vector3 targetStartPosition;
    private Vector3 selfStartPosition;
    private bool isInitialized = false; // Trava para garantir que a inicialização ocorra apenas uma vez

    // O método Start agora apenas inicia a corrotina.
    IEnumerator Start()
    {
        if (targetToFollow == null)
        {
            Debug.LogError("Alvo para o Parallax não foi definido!", this.gameObject);
            this.enabled = false;
            yield break; // Para a execução da corrotina
        }
        
        // Espera pelo final do primeiro frame.
        yield return new WaitForEndOfFrame();
        
        // Agora que a câmera já se posicionou, capturamos as posições corretas.
        targetStartPosition = targetToFollow.position;
        selfStartPosition = transform.position;
        isInitialized = true;
    }

    void LateUpdate()
    {
        // Só executa a lógica de parallax depois que a inicialização estiver completa.
        if (!isInitialized) return;

        float deltaX = targetToFollow.position.x - targetStartPosition.x;
        float newPositionX = selfStartPosition.x - (deltaX * parallaxMultiplierX);

        transform.position = new Vector3(newPositionX, selfStartPosition.y, selfStartPosition.z);
    }
}