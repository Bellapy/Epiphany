using UnityEngine;

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
        if (isInteracting) return;
        isInteracting = true;

        // <<< CORREÇÃO APLICADA AQUI >>>
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.StartClimbingSequence(startClimbPoint, endClimbPoint, nextSceneName, spawnPointInNextScene);
        }
    }
}