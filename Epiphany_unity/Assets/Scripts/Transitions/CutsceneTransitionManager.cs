using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneTransitionManager : MonoBehaviour
{
    [Header("Referências da UI (Opcionais)")]
    public CanvasGroup fadePanelCanvasGroup;
    public GameObject pixelArtDisplayImageObject;
    public RectTransform pixelArtImageRectTransform;

    [Header("Referências da Animação (Opcionais)")]
    public SpriteRenderer animatedCutsceneSprite;

    [Header("Referências de Áudio (Opcionais)")]
    public AudioSource musicAudioSource;

    [Header("Configurações de Tempo")]
    public float delayAfterTextFinishes = 2.0f;
    public float animationFadeOutDuration = 1.5f;
    public float initialFadeToBlackDuration = 1.0f;
    public float revealPixelArtDuration = 0.5f;
    public float pixelArtDisplayTime = 5.0f;
    public float zoomStartScale = 1.0f;
    public float zoomEndScale = 1.2f;
    public float musicFadeOutDuration = 4.0f;
    public float finalFadeToBlackDuration = 1.0f;
    public string sceneNameToLoad = ""; // Deixe vazio se não for carregar cena

    private Coroutine activeTransitionCoroutine;

    // Método público que o SequentialTypewriter vai chamar
    public void StartPostTextSequence()
    {
        if (activeTransitionCoroutine != null) StopCoroutine(activeTransitionCoroutine);
        activeTransitionCoroutine = StartCoroutine(PostTextSequenceCoroutine());
    }

    IEnumerator PostTextSequenceCoroutine()
    {
        // Fade out da animação (só se existir)
        if (animatedCutsceneSprite != null)
        {
            StartCoroutine(FadeSpriteRenderer(animatedCutsceneSprite, animatedCutsceneSprite.color.a, 0f, animationFadeOutDuration));
        }

        // Espera
        yield return new WaitForSeconds(delayAfterTextFinishes);

        // Fade para preto (só se existir)
        if (fadePanelCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(fadePanelCanvasGroup, fadePanelCanvasGroup.alpha, 1f, initialFadeToBlackDuration));
        }

        // Mostra imagem e zoom (só se existir)
        if (pixelArtDisplayImageObject != null)
        {
            pixelArtDisplayImageObject.SetActive(true);
            if (pixelArtImageRectTransform != null)
            {
                pixelArtImageRectTransform.localScale = new Vector3(zoomStartScale, zoomStartScale, 1f);
                StartCoroutine(ZoomImage(pixelArtImageRectTransform, zoomStartScale, zoomEndScale, pixelArtDisplayTime));
            }

            if (fadePanelCanvasGroup != null)
            {
                yield return StartCoroutine(FadeCanvasGroup(fadePanelCanvasGroup, 1f, 0f, revealPixelArtDuration));
            }

            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                StartCoroutine(FadeOutAudio(musicAudioSource, musicFadeOutDuration));
            }
            
            yield return new WaitForSeconds(pixelArtDisplayTime);

            if (fadePanelCanvasGroup != null)
            {
                yield return StartCoroutine(FadeCanvasGroup(fadePanelCanvasGroup, fadePanelCanvasGroup.alpha, 1f, finalFadeToBlackDuration));
            }
        }

        // Carrega a cena (só se tiver um nome)
        if (!string.IsNullOrEmpty(sceneNameToLoad))
        {
            SceneManager.LoadSceneAsync(sceneNameToLoad);
        }
    }
    
    // Suas outras corrotinas (FadeCanvasGroup, etc.) permanecem aqui...
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
    IEnumerator FadeSpriteRenderer(SpriteRenderer sr, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = sr.color; 
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            sr.color = color;
            yield return null;
        }
        color.a = endAlpha;
        sr.color = color;
        if (endAlpha == 0 && sr != null)
        {
            sr.gameObject.SetActive(false); 
        }
    }
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
    IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        if (audioSource == null) yield break;

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
    }
}