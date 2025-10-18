using UnityEngine;
using UnityEngine.UI; // Essencial para ter acesso ao componente 'Image'
using System;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [Header("Referências do Painel")]
    [Tooltip("Arraste aqui o CanvasGroup do seu painel de fade.")]
    [SerializeField] private CanvasGroup fadePanelCanvasGroup;
    
    [Tooltip("Arraste a Imagem do seu painel de fade aqui (para mudança de cor).")]
    [SerializeField] private Image fadePanelImage;

    [Header("Configurações de Tempo")]
    [Tooltip("Duração padrão do fade em segundos.")]
    public float fadeDuration = 1.0f; // Variável agora é pública para ser acessada por outros scripts

    public static FadeController Instance { get; private set; }

    private Coroutine currentFadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Se este objeto precisar persistir entre as cenas, descomente a linha abaixo.
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (fadePanelCanvasGroup == null)
        {
            Debug.LogError("[FadeController] ERRO: O 'Fade Panel Canvas Group' não foi atribuído no Inspector!");
            this.enabled = false;
            return;
        }
        
        fadePanelCanvasGroup.alpha = 0f;
        fadePanelCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// Inicia um fade para uma cor (preto por padrão), tornando a tela opaca.
    /// </summary>
    /// <param name="onComplete">Ação a ser executada quando o fade terminar.</param>
    /// <param name="fadeColor">A cor para a qual a tela fará o fade. Se nulo, usa preto.</param>
    public void StartFadeOut(Action onComplete = null, Color? fadeColor = null)
    {
        if (!this.enabled) return;

        if (fadePanelImage != null)
        {
            fadePanelImage.color = fadeColor ?? Color.black;
        }
        else
        {
            Debug.LogWarning("[FadeController] Fade Panel Image não atribuído. Usando a cor existente.");
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        fadePanelCanvasGroup.gameObject.SetActive(true);
        currentFadeCoroutine = StartCoroutine(FadeRoutine(1f, onComplete));
    }

    /// <summary>
    /// Inicia um fade a partir de uma tela opaca, tornando a cena visível.
    /// </summary>
    /// <param name="onComplete">Ação a ser executada quando o fade terminar.</param>
    public void StartFadeIn(Action onComplete = null)
    {
        if (!this.enabled) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        fadePanelCanvasGroup.alpha = 1f;
        fadePanelCanvasGroup.gameObject.SetActive(true);
        currentFadeCoroutine = StartCoroutine(FadeRoutine(0f, () => {
            fadePanelCanvasGroup.gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    /// <summary>
    /// A rotina interna que interpola o valor alpha do CanvasGroup.
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