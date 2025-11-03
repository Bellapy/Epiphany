using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Se este UIManager estiver em um objeto raiz, você pode adicionar DontDestroyOnLoad(gameObject);
            // Se ele estiver no Player (que já é persistente), não precisa.
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private CanvasGroup interactionPromptCanvasGroup;
    private Coroutine promptFadeCoroutine;

    public void ConnectUI(UIDialogueConnector connector)
    {
        // Se já temos uma referência, não fazemos nada.
        // Isso evita que um UIDialogueConnector de uma cena não-persistente sobrescreva a conexão.
        if (interactionPromptCanvasGroup != null) return;

        interactionPromptCanvasGroup = connector.interactionPromptCanvasGroup;
        if(interactionPromptCanvasGroup != null)
        {
            Debug.Log("<color=green>[UIManager.ConnectUI] Conexão bem-sucedida. CanvasGroup do prompt recebido.</color>");
            interactionPromptCanvasGroup.alpha = 0;
        }
        else
        {
            Debug.LogWarning("<color=yellow>[UIManager.ConnectUI] Conexão recebida, mas o CanvasGroup no Connector está NULO!</color>");
        }
    }

    public void ShowInteractionPrompt()
    {
        if (interactionPromptCanvasGroup == null)
        {
            Debug.LogError("[UIManager] Tentou mostrar o prompt, mas a referência do CanvasGroup é NULA. A conexão inicial falhou ou foi perdida.");
            return;
        }

        if (promptFadeCoroutine != null) StopCoroutine(promptFadeCoroutine);
        promptFadeCoroutine = StartCoroutine(FadePrompt(1f));
    }

    public void HideInteractionPrompt()
    {
        if (interactionPromptCanvasGroup == null)
        {
            return;
        }

        if (promptFadeCoroutine != null) StopCoroutine(promptFadeCoroutine);
        promptFadeCoroutine = StartCoroutine(FadePrompt(0f));
    }
    
    private IEnumerator FadePrompt(float targetAlpha)
    {
        if (interactionPromptCanvasGroup == null) yield break;

        float startAlpha = interactionPromptCanvasGroup.alpha;
        float timer = 0f;
        float duration = 0.3f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            interactionPromptCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        interactionPromptCanvasGroup.alpha = targetAlpha;
    }
}