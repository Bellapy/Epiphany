using UnityEngine;
using System.Collections; // Adicionado para a corrotina

public class AylaStorySceneController : MonoBehaviour
{
    private FadeController fadeController;
    private int completedStations = 0;
    private const int TOTAL_STATIONS = 7;

    private void Start()
    {
        // Encontra o FadeController que existe na cena (do seu Canvas persistente)
        fadeController = FindFirstObjectByType<FadeController>();
    }

    // Esta função será chamada pelo UnityEvent OnStationCompleted de cada StoryStationController
    public void MarkStationAsCompleted()
    {
        completedStations++;
        if (completedStations >= TOTAL_STATIONS)
        {
            // Todas as estações foram concluídas, inicia a transição final
            StartCoroutine(FinalTransition());
        }
    }

    private IEnumerator FinalTransition()
    {
        // 1. Espera 1 segundo após a última estação terminar.
        yield return new WaitForSeconds(1.0f);

        // 2. Chama o FadeController para iniciar o fade-out.
        if (fadeController != null)
        {
            // A cor preta é o padrão, então não precisamos especificá-la.
            fadeController.StartFadeOut(() => {
                // 3. Esta parte só é executada QUANDO o fade terminar.
                GameManager.Instance.LoadScene("confrontamento");
            });
        }
        else
        {
            // Fallback: Se o FadeController não for encontrado, carrega a cena imediatamente.
            Debug.LogWarning("FadeController não encontrado. Carregando a cena diretamente.");
            GameManager.Instance.LoadScene("confrontamento");
        }
    }
}