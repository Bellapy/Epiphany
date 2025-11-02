using UnityEngine;
using System.Collections;

public class EndingSceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [Tooltip("O objeto que contém o script StaticTypewriter.")]
    [SerializeField] private StaticTypewriter typewriter;
    [Tooltip("O texto completo a ser escrito na tela.")]
    [TextArea(3, 10)]
    [SerializeField] private string endingText;

    [Header("Configurações de Timing")]
    [Tooltip("Atraso em segundos antes de o texto começar a ser escrito.")]
    [SerializeField] private float initialDelay = 2.0f;
    [Tooltip("Atraso em segundos após o texto terminar, antes do fade-out.")]
    [SerializeField] private float finalDelay = 5.0f;

    private FadeController fadeController;

    private void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // 1. Espera inicial
        yield return new WaitForSeconds(initialDelay);

        // 2. Inicia a digitação e se inscreve no evento de conclusão
        if (typewriter != null)
        {
            // Adiciona um "ouvinte" que será chamado quando a digitação terminar
            typewriter.OnTypingCompleted.AddListener(HandleTypingCompleted);
            typewriter.StartTyping(endingText);
        }
    }

    // 3. Esta função é chamada pelo evento OnTypingCompleted do StaticTypewriter
    private void HandleTypingCompleted()
    {
        // Inicia a corrotina final após a digitação
        StartCoroutine(FinalTransition());
    }

    private IEnumerator FinalTransition()
    {
        // 4. Espera final
        yield return new WaitForSeconds(finalDelay);

        // 5. Inicia o fade-out e retorna ao menu
        if (fadeController != null)
        {
            fadeController.StartFadeOut(() => {
                GameManager.Instance.LoadScene("menu2");
            });
        }
        else
        {
            GameManager.Instance.LoadScene("menu2");
        }
    }
}