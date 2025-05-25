// FadeController.cs
using UnityEngine;
using System.Collections;
using System; // Necessário para Action

public class FadeController : MonoBehaviour
{
    [Header("Configurações do Fade")]
    [Tooltip("O CanvasGroup do painel que será usado para o fade.")]
    public CanvasGroup fadePanelCanvasGroup;

    [Tooltip("Duração do fade em segundos.")]
    public float fadeDuration = 1.0f;

    private Coroutine currentFadeCoroutine;

    // Método para garantir que só haja uma instância (Singleton Simples)
    // Isso facilita o acesso de outros scripts
    public static FadeController Instance { get; private set; }

    void Awake()
    {
        // Configuração do Singleton
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Descomente se você quiser que ele persista entre cenas (provavelmente não para este caso de menu)
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Garante que o painel esteja configurado corretamente no início
        if (fadePanelCanvasGroup != null)
        {
            fadePanelCanvasGroup.alpha = 0f; // Começa invisível
            fadePanelCanvasGroup.gameObject.SetActive(true); // Garante que está ativo para o fade
        }
        else
        {
            Debug.LogError("FadeController: CanvasGroup do painel de fade não foi atribuído no Inspector!");
        }
    }

    /// <summary>
    /// Inicia o processo de fade out (tela escurece).
    /// Chama onComplete quando o fade termina.
    /// </summary>
    public void StartFadeOut(Action onComplete = null)
    {
        if (fadePanelCanvasGroup == null)
        {
            Debug.LogError("FadeController: Não é possível iniciar o fade out, CanvasGroup não atribuído.");
            onComplete?.Invoke(); // Chama onComplete imediatamente se não puder fazer o fade
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeRoutine(1f, onComplete)); // 1f para fade out
    }

    /// <summary>
    /// Inicia o processo de fade in (tela clareia).
    /// Chama onComplete quando o fade termina.
    /// </summary>
    public void StartFadeIn(Action onComplete = null)
    {
        if (fadePanelCanvasGroup == null)
        {
            Debug.LogError("FadeController: Não é possível iniciar o fade in, CanvasGroup não atribuído.");
            onComplete?.Invoke();
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeRoutine(0f, onComplete)); // 0f para fade in
    }

    private IEnumerator FadeRoutine(float targetAlpha, Action onComplete)
    {
        float startAlpha = fadePanelCanvasGroup.alpha;
        float elapsedTime = 0f;

        fadePanelCanvasGroup.gameObject.SetActive(true); // Garante que o painel está ativo

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanelCanvasGroup.alpha = newAlpha;
            yield return null;
        }

        fadePanelCanvasGroup.alpha = targetAlpha;
        currentFadeCoroutine = null;

        onComplete?.Invoke(); // Chama a ação de callback (onComplete)
    }
}