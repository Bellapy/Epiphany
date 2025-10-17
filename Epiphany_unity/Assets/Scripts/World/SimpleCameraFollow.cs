using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("Configuração do Follow")]
    [Tooltip("Arraste o Transform do jogador para este campo.")]
    [SerializeField] private Transform target;
    [Tooltip("A suavidade do movimento da câmera. Valores maiores tornam o movimento mais rápido e rígido.")]
    [SerializeField] private float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 1. Define a posição X desejada (a mesma do jogador).
        float desiredXPosition = target.position.x;

        // 2. Calcula a nova posição X da câmera de forma suave usando Lerp.
        float smoothedXPosition = Mathf.Lerp(transform.position.x, desiredXPosition, smoothSpeed * Time.deltaTime);

        // 3. Monta a nova posição da câmera, mantendo os eixos Y e Z originais (travados).
        Vector3 newCameraPosition = new Vector3(smoothedXPosition, transform.position.y, transform.position.z);

        // 4. Aplica a nova posição à câmera.
        transform.position = newCameraPosition;
    }
}