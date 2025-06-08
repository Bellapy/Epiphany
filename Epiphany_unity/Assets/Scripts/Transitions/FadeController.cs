using UnityEngine;
using System.Collections;
using System;

public class FadeController : MonoBehaviour
{
    [Header("Configurações do Fade")]
    [Tooltip("O CanvasGroup do painel que será usado para o fade.")]
    public CanvasGroup fadePanelCanvasGroup;

    [Tooltip("Duração do fade em segundos.")]
    public float fadeDuration = 1.0f;

    private Coroutine currentFadeCoroutine;

    // Singleton para fácil acesso
    public static FadeController Instance { get; private set; }

    void Awake()
    {
        // Configuração do Singleton
        if (Instance == null)
        {
            Instance = this;
            // Se quiser que persista entre cenas, descomente a linha abaixo
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (fadePanelCanvasGroup == null)
        {
            Debug.LogError("FadeController: CanvasGroup do painel de fade não foi atribuído no Inspector!");
            return;
        }

        // Inicializa o painel transparente e ativo
        fadePanelCanvasGroup.alpha = 0f;
        fadePanelCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// Faz a tela escurecer (fade out).
    /// </summary>
    public void StartFadeOut(Action onComplete = null)
    {
        if (fadePanelCanvasGroup == null)
        {
            Debug.LogError("FadeController: CanvasGroup não atribuído.");
            onComplete?.Invoke();
            return;
        }

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        // Ativa o painel e inicia fade para alpha = 1 (preto opaco)
        fadePanelCanvasGroup.gameObject.SetActive(true);
        currentFadeCoroutine = StartCoroutine(FadeRoutine(1f, onComplete));
    }

    /// <summary>
    /// Faz a tela clarear (fade in).
    /// </summary>
    public void StartFadeIn(Action onComplete = null)
    {
        if (fadePanelCanvasGroup == null)
        {
            Debug.LogError("FadeController: CanvasGroup não atribuído.");
            onComplete?.Invoke();
            return;
        }

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        // Para o fade in funcionar, o painel deve estar visível e com alpha 1
        fadePanelCanvasGroup.alpha = 1f;
        fadePanelCanvasGroup.gameObject.SetActive(true);
        currentFadeCoroutine = StartCoroutine(FadeRoutine(0f, () =>
        {
            // Após fade in, desativa o painel para melhorar performance
            fadePanelCanvasGroup.gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    /// <summary>
    /// Coroutine que faz a interpolação do alpha.
    /// </summary>
    private IEnumerator FadeRoutine(float targetAlpha, Action onComplete)
    {
        float startAlpha = fadePanelCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanelCanvasGroup.alpha = newAlpha;
            yield return null;
        }

        fadePanelCanvasGroup.alpha = targetAlpha;
        currentFadeCoroutine = null;
        onComplete?.Invoke();
    }
}
