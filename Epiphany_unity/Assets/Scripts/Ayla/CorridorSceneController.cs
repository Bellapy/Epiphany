// Em Scripts/CorridorSceneController.cs (ou crie-o se não existir)

using UnityEngine;

public class CorridorSceneController : MonoBehaviour
{
    [Header("Referências de Progressão")]
    [Tooltip("Arraste aqui o GameObject da barreira que bloqueia o caminho para a floresta.")]
    [SerializeField] private GameObject barreiraFloresta;

    void Start()
    {
        if (barreiraFloresta == null)
        {
            Debug.LogError("[CorridorSceneController] A referência para 'Barreira Floresta' não foi configurada!");
            return;
        }

        // Verifica o estado no GameManager
        if (GameManager.Instance != null && GameManager.Instance.HasSolvedCorridorPuzzle)
        {
            // Se o puzzle foi resolvido, desativa a barreira.
            barreiraFloresta.SetActive(false);
        }
        else
        {
            // Se não, garante que a barreira esteja ativa.
            barreiraFloresta.SetActive(true);
        }
    }
}