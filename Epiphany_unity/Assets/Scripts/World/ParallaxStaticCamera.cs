using UnityEngine;

public class ParallaxStaticCamera : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Arraste o Transform do seu Player aqui.")]
    [SerializeField] private Transform targetToFollow;

    [Header("Configuração do Efeito")]
    [Tooltip("A força do efeito. Valores pequenos (ex: 0.05) para camadas distantes, valores maiores (ex: 0.2) para camadas próximas.")]
    [SerializeField] private float parallaxStrength = 0.1f;

    private Vector3 initialPosition;

    void Start()
    {
        if (targetToFollow == null)
        {
            Debug.LogError("Alvo para o Parallax não foi definido!", this.gameObject);
            this.enabled = false;
            return;
        }
        
        initialPosition = transform.position;
    }

    void LateUpdate()
    {
        float displacementX = targetToFollow.position.x * parallaxStrength;

        transform.position = new Vector3(initialPosition.x - displacementX, initialPosition.y, initialPosition.z);
    }
}