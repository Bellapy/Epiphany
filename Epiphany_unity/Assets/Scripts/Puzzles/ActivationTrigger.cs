using UnityEngine;

public class ActivationTrigger : MonoBehaviour
{
    // Arraste o seu objeto "CrystalPuzzleManager" para cá no Inspector
    [SerializeField] private CrystalPuzzleManager puzzleManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou na zona do puzzle. Ativando Modo Puzzle.");
            // Pega o componente do player para pará-lo
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.DisableMovement();
            }

            // Ativa o puzzle
            if (puzzleManager != null)
            {
                puzzleManager.ActivatePuzzle();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player saiu da zona do puzzle. Desativando Modo Puzzle.");
            // Pega o componente do player para reativar o movimento
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.EnableMovement();
            }

            // Desativa o puzzle
            if (puzzleManager != null)
            {
                puzzleManager.DeactivatePuzzle();
            }
        }
    }
}