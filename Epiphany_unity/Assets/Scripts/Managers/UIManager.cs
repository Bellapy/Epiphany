using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Referência privada, preenchida pelo UIDialogueConnector de cada cena
    private CanvasGroup interactionPromptCanvasGroup;
    private Coroutine promptFadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Limpa as referências ao carregar uma nova cena para evitar que aponte para objetos destruídos.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        interactionPromptCanvasGroup = null;
        if(promptFadeCoroutine != null)
        {
            StopCoroutine(promptFadeCoroutine);
            promptFadeCoroutine = null;
        }
    }

    /// <summary>
    /// Recebe as referências da UI da cena atual através do UIDialogueConnector.
    /// </summary>
    public void ConnectUI(UIDialogueConnector connector)
    {
        interactionPromptCanvasGroup = connector.interactionPromptCanvasGroup;
        if(interactionPromptCanvasGroup != null)
        {
            interactionPromptCanvasGroup.alpha = 0; // Garante que comece invisível
        }
    }

    /// <summary>
    /// Mostra o prompt de interação com um efeito de fade.
    /// </summary>
    public void ShowInteractionPrompt()
    {
        // VERIFICAÇÃO DE SEGURANÇA: Só tenta mostrar o prompt se ele existir nesta cena.
        if (interactionPromptCanvasGroup == null)
        {
            return;
        }

        if (promptFadeCoroutine != null) StopCoroutine(promptFadeCoroutine);
        promptFadeCoroutine = StartCoroutine(FadePrompt(1f));
    }

    /// <summary>
    /// Esconde o prompt de interação com um efeito de fade.
    /// </summary>
    public void HideInteractionPrompt()
    {
        // VERIFICAÇÃO DE SEGURANÇA: Só tenta esconder o prompt se ele existir nesta cena.
        if (interactionPromptCanvasGroup == null)
        {
            return;
        }

        if (promptFadeCoroutine != null) StopCoroutine(promptFadeCoroutine);
        promptFadeCoroutine = StartCoroutine(FadePrompt(0f));
    }
    
    /// <summary>
    /// Coroutine que controla o efeito de fade (aparecer/desaparecer) do prompt.
    /// </summary>
    private IEnumerator FadePrompt(float targetAlpha)
    {
        // Verificação extra dentro da coroutine, por segurança.
        if (interactionPromptCanvasGroup == null) yield break;

        float startAlpha = interactionPromptCanvasGroup.alpha;
        float timer = 0f;
        float duration = 0.3f; // Duração do fade em segundos

        while (timer < duration)
        {
            timer += Time.deltaTime;
            interactionPromptCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        interactionPromptCanvasGroup.alpha = targetAlpha;
    }
}