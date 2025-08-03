using UnityEngine;

// Este script precisa ser 'IInteractable' para que o PlayerInteractor o detecte.
public class StairTrigger : MonoBehaviour, IInteractable
{
    [Header("Configurações da Sequência")]
    [Tooltip("Ponto exato onde a personagem deve ficar antes de subir.")]
    [SerializeField] private Transform startClimbPoint;
    
    [Tooltip("Ponto final para onde a personagem se moverá ao subir.")]
    [SerializeField] private Transform endClimbPoint;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string spawnPointInNextScene;

    private bool isInteracting = false;

    public void Interact()
    {
        // Impede múltiplas interações
        if (isInteracting) return;
        isInteracting = true;

        // Encontra o PlayerController na cena e dá a ordem para iniciar a subida.
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.StartClimbingSequence(startClimbPoint, endClimbPoint, nextSceneName, spawnPointInNextScene);
        }
    }
}