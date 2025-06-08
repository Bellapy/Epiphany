using UnityEngine;
using UnityEngine.UI; // Ainda necessário para o fadePanelCanvasGroup e pixelArtImage
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneTransitionManager : MonoBehaviour
{
    [Header("Referências da UI para Transição")]
    public CanvasGroup fadePanelCanvasGroup;       // Arraste o CanvasGroup do seu FadePanel aqui
    public GameObject pixelArtDisplayImageObject;  // Arraste o GameObject da sua PixelArtDisplayImage aqui
    public RectTransform pixelArtImageRectTransform; // Arraste o RectTransform da PixelArtDisplayImage aqui

    [Header("Referências da Animação da Cutscene")]
    public SpriteRenderer animatedCutsceneSprite; // Arraste o SpriteRenderer da sua animação aqui

    [Header("Referências de Áudio")]
    public AudioSource musicAudioSource;          // Arraste o AudioSource com a música da cutscene aqui

    [Header("Configurações de Tempo e Efeitos")]
    public float delayAfterTextFinishes = 2.0f;     // Espera antes do fade para preto da tela
    public float animationFadeOutDuration = 1.5f; // Duração do fade da animação da cutscene
    public float initialFadeToBlackDuration = 1.0f; // Fade da tela para preto
    public float revealPixelArtDuration = 0.5f;   // Tempo para o fade preto sair e revelar a imagem
    public float pixelArtDisplayTime = 5.0f;    // Quanto tempo a imagem com zoom fica na tela
    public float zoomStartScale = 1.0f;
    public float zoomEndScale = 1.2f;
    public float musicFadeOutDuration = 4.0f;
    public float finalFadeToBlackDuration = 1.0f; // Fade da tela para preto antes de carregar a cena
    public string sceneNameToLoad = "Scene1";

    private Coroutine activeTransitionCoroutine;
    private float initialMusicVolume;

    void Start()
    {
        // Validação das referências essenciais
        if (fadePanelCanvasGroup == null) { Debug.LogError("FadePanel CanvasGroup não atribuído!"); enabled = false; return; }
        if (pixelArtDisplayImageObject == null) { Debug.LogError("PixelArtDisplayImage GameObject não atribuído!"); enabled = false; return; }
        if (pixelArtImageRectTransform == null) { Debug.LogError("PixelArtImage RectTransform não atribuído!"); enabled = false; return; }

        // Validação das referências opcionais mas importantes para a funcionalidade completa
        if (animatedCutsceneSprite == null)
        {
            Debug.LogWarning("SpriteRenderer da animação da cutscene não atribuído. O fade da animação não funcionará.");
        }
        if (musicAudioSource == null)
        {
            Debug.LogWarning("Music AudioSource não atribuído. Fade de música não funcionará.");
        }
        else
        {
            initialMusicVolume = musicAudioSource.volume;
        }

        // Estado inicial
        fadePanelCanvasGroup.alpha = 0f;
        fadePanelCanvasGroup.blocksRaycasts = false;
        pixelArtDisplayImageObject.SetActive(false);
        pixelArtImageRectTransform.localScale = new Vector3(zoomStartScale, zoomStartScale, 1f);
    }

    public void StartPostTextSequence()
    {
        if (activeTransitionCoroutine != null)
        {
            StopCoroutine(activeTransitionCoroutine);
        }
        activeTransitionCoroutine = StartCoroutine(PostTextSequenceCoroutine());
    }

    IEnumerator PostTextSequenceCoroutine()
    {
        // 0. Iniciar o fade out da animação da cutscene (SpriteRenderer)
        // Este fade começa assim que os textos terminam.
        if (animatedCutsceneSprite != null)
        {
            StartCoroutine(FadeSpriteRenderer(animatedCutsceneSprite, animatedCutsceneSprite.color.a, 0f, animationFadeOutDuration));
        }

        // 1. Pequena Espera (enquanto a animação da cutscene faz fade out em paralelo)
        yield return new WaitForSeconds(delayAfterTextFinishes);

        // 2. Fade da Tela para Preto (cobre a cutscene atual, incluindo a animação que está sumindo)
        Debug.Log("Iniciando Fade da Tela para Preto...");
        fadePanelCanvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(FadeCanvasGroup(fadePanelCanvasGroup, fadePanelCanvasGroup.alpha, 1f, initialFadeToBlackDuration));

        // 3. Imagem Pixel Art Surge
        Debug.Log("Mostrando Imagem Pixel Art...");
        pixelArtDisplayImageObject.SetActive(true);
        pixelArtImageRectTransform.localScale = new Vector3(zoomStartScale, zoomStartScale, 1f);

        // O painel preto some para revelar a imagem pixel art
        yield return StartCoroutine(FadeCanvasGroup(fadePanelCanvasGroup, 1f, 0f, revealPixelArtDuration));
        fadePanelCanvasGroup.blocksRaycasts = false;

        // 4. Iniciar Zoom Lento na Imagem e Fade Out da Música
        Debug.Log("Iniciando Zoom na Imagem e Fade Out da Música...");
        StartCoroutine(ZoomImage(pixelArtImageRectTransform, zoomStartScale, zoomEndScale, pixelArtDisplayTime));

        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            StartCoroutine(FadeOutAudio(musicAudioSource, musicFadeOutDuration));
        }

        // Espera o tempo da imagem em tela
        yield return new WaitForSeconds(pixelArtDisplayTime);

        // 5. Fade da Tela para Preto Novamente (cobrindo a imagem pixel art)
        Debug.Log("Iniciando transição para a próxima cena...");
        fadePanelCanvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(FadeCanvasGroup(fadePanelCanvasGroup, fadePanelCanvasGroup.alpha, 1f, finalFadeToBlackDuration));

        // 6. Carrega a Próxima Cena
        Debug.Log($"Carregando Cena: {sceneNameToLoad}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        activeTransitionCoroutine = null;
        Debug.Log("Transição completa. Nova cena carregada.");
    }

    // Corrotina para fade de CanvasGroup (para o painel de fade da tela)
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    // Corrotina para fade de SpriteRenderer (para a animação da cutscene)
    IEnumerator FadeSpriteRenderer(SpriteRenderer sr, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = sr.color; // Pega a cor atual para preservar R, G, B

        // Se startAlpha não for fornecido ou for diferente do alfa atual, use o alfa atual do sprite.
        // No nosso caso, estamos passando sr.color.a, então está correto.
        // color.a = startAlpha; // Desnecessário se já passamos o alfa atual.

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            sr.color = color;
            yield return null;
        }
        color.a = endAlpha;
        sr.color = color;

        if (endAlpha == 0 && sr != null) // Se o fade foi para totalmente transparente
        {
            sr.gameObject.SetActive(false); // Opcional: desativar o GameObject
        }
    }

    // Corrotina para zoom da imagem pixel art
    IEnumerator ZoomImage(RectTransform imageRect, float startScale, float endScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = new Vector3(startScale, startScale, 1f);
        Vector3 targetScale = new Vector3(endScale, endScale, 1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            imageRect.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            yield return null;
        }
        imageRect.localScale = targetScale;
    }

    // Corrotina para fade out do áudio
    IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        if (audioSource == null) yield break; // Segurança

        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        // Considerar se quer resetar o volume aqui ou deixar para quem for usar o AudioSource depois.
        // Se este AudioSource só toca esta música, resetar é bom.
        // audioSource.volume = initialMusicVolume; // Comentado para evitar ligar o som se não desejado
    }
}