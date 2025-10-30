using UnityEngine;
using System.Collections.Generic;

public class AylaStorySceneController : MonoBehaviour
{
    [Header("Configuração da Sequência")]
    [SerializeField] private List<StoryFragmentController> storyFragments;
    [SerializeField] private string nextSceneName = "confrontamento";

    private int currentFragmentIndex = 0;
    private FadeController fadeController;

    private void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();

        // Conecta o evento de cada fragmento a este controlador
        foreach (var fragment in storyFragments)
        {
            fragment.OnFragmentCompleted.AddListener(OnFragmentCompleted);
        }
    }

    private void OnFragmentCompleted()
    {
        currentFragmentIndex++;

        // Verifica se todos os fragmentos foram concluídos
        if (currentFragmentIndex >= storyFragments.Count)
        {
            StartSceneTransition();
        }
    }

    private void StartSceneTransition()
    {
        if (fadeController != null && GameManager.Instance != null)
        {
            fadeController.StartFadeOut(() => {
                GameManager.Instance.LoadScene(nextSceneName);
            });
        }
    }
}