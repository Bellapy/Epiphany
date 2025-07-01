// Em _Scripts/Managers/UIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // --- Configurações de Timing (Ajustáveis no Inspector) ---
    [Header("Configurações Globais de Timing")]
    [Tooltip("Tempo em segundos entre cada letra. Menor = mais rápido.")]
    [SerializeField] private float timePerCharacter = 0.02f;
    [Tooltip("Pausa em segundos entre as frases, se houver mais de uma na mesma interação.")]
    [SerializeField] private float delayBetweenLines = 1.5f;
    [Tooltip("Tempo que o painel fica na tela APÓS o texto terminar, antes de sumir.")]
    [SerializeField] private float timeOnScreenAfterTyping = 3.0f;
    [Tooltip("Duração do fade in/out dos painéis em segundos.")]
    [SerializeField] private float fadeDuration = 0.3f;

    // --- Referências Internas (Conectadas a cada cena) ---
    private GameObject currentBackgroundPanel;
    private CanvasGroup currentPanelCanvasGroup;
    private TextMeshProUGUI currentReflectionText;
    private Image currentPortraitImage;
    private GameObject currentPortraitContainer;
    private Sprite currentPlayerPortrait;
    private CanvasGroup interactionPromptCanvasGroup;

    // --- Variáveis de Estado Internas ---
    private Coroutine activeDialogueCoroutine;
    private Coroutine promptFadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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

    // Limpa tudo ao carregar uma nova cena para evitar erros de referência
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        activeDialogueCoroutine = null;
        promptFadeCoroutine = null;
        // Limpa as referências para garantir que o próximo Connector funcione corretamente
        currentBackgroundPanel = null;
        currentPanelCanvasGroup = null;
        interactionPromptCanvasGroup = null;
    }

    public void ConnectUI(UIDialogueConnector connector)
    {
        currentBackgroundPanel = connector.backgroundPanel;
        if (currentBackgroundPanel != null)
        {
            currentPanelCanvasGroup = currentBackgroundPanel.GetComponent<CanvasGroup>();
        }
        currentReflectionText = connector.reflectionText;
        currentPortraitImage = connector.portraitImage;
        currentPortraitContainer = connector.portraitContainer;
        currentPlayerPortrait = connector.playerPortrait;
        interactionPromptCanvasGroup = connector.interactionPromptCanvasGroup;

        if (currentBackgroundPanel != null)
        {
            currentBackgroundPanel.SetActive(false);
            if(currentPanelCanvasGroup != null) currentPanelCanvasGroup.alpha = 0;
        }
        if (interactionPromptCanvasGroup != null)
        {
            interactionPromptCanvasGroup.alpha = 0;
        }
    }

    // --- MÉTODOS PÚBLICOS PARA CONTROLE EXTERNO ---

    public void ShowEnvironmentalReflection(ReflectionData data)
    {
        if (currentPortraitContainer != null) currentPortraitContainer.SetActive(false);
        StartShowingText(data.reflectionLines);
    }

    public void ShowPersonalReflection(ReflectionData data)
    {
        if (currentPortraitImage != null && currentPlayerPortrait != null && currentPortraitContainer != null)
        {
            currentPortraitImage.sprite = currentPlayerPortrait;
            currentPortraitContainer.SetActive(true);
        }
        StartShowingText(data.reflectionLines);
    }

    public void HideReflection()
    {
        if (currentBackgroundPanel != null && currentBackgroundPanel.activeSelf)
        {
            if (activeDialogueCoroutine != null) StopCoroutine(activeDialogueCoroutine);
            activeDialogueCoroutine = StartCoroutine(FadeOutDialogueRoutine());
        }
    }

    public void ShowInteractionPrompt()
    {
        if (promptFadeCoroutine != null) StopCoroutine(promptFadeCoroutine);
        promptFadeCoroutine = StartCoroutine(FadePrompt(1f));
    }

    public void HideInteractionPrompt()
    {
        if (promptFadeCoroutine != null) StopCoroutine(promptFadeCoroutine);
        promptFadeCoroutine = StartCoroutine(FadePrompt(0f));
    }
    
    // --- LÓGICA INTERNA E CORROTINAS ---
    
    private void StartShowingText(List<string> lines)
    {
        if (currentBackgroundPanel == null)
        {
            Debug.LogWarning("UIManager tentou mostrar texto, mas o Background Panel não está conectado.");
            return;
        }
        
        if (currentReflectionText != null) currentReflectionText.text = "";
        
        if (activeDialogueCoroutine != null) StopCoroutine(activeDialogueCoroutine);
        activeDialogueCoroutine = StartCoroutine(DialogueRoutine(lines));
    }

    private IEnumerator DialogueRoutine(List<string> lines)
    {
        currentBackgroundPanel.SetActive(true);
        yield return StartCoroutine(FadeRoutine(currentPanelCanvasGroup, 1f));

        for (int i = 0; i < lines.Count; i++)
        {
            yield return StartCoroutine(TypeSentence(lines[i]));
            if (i < lines.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenLines); 
            }
        }
        
        yield return new WaitForSeconds(timeOnScreenAfterTyping);
        
        yield return StartCoroutine(FadeOutDialogueRoutine());
    }

    private IEnumerator FadeOutDialogueRoutine()
    {
        yield return StartCoroutine(FadeRoutine(currentPanelCanvasGroup, 0f));
        if (currentBackgroundPanel != null)
        {
            currentBackgroundPanel.SetActive(false);
        }
        activeDialogueCoroutine = null;
    }

    private IEnumerator FadePrompt(float targetAlpha)
    {
        yield return StartCoroutine(FadeRoutine(interactionPromptCanvasGroup, targetAlpha));
    }

    private IEnumerator FadeRoutine(CanvasGroup group, float targetAlpha)
    {
        if (group == null)
        {
            // Este aviso agora é esperado em cenas com UI simples, então podemos remover ou comentar
            // Debug.LogWarning("Tentando fazer fade, mas não há CanvasGroup no painel conectado.");
            yield break;
        }

        float startAlpha = group.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        group.alpha = targetAlpha;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        if (currentReflectionText == null) yield break;
        currentReflectionText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            currentReflectionText.text += letter;
            yield return new WaitForSeconds(timePerCharacter); 
        }
    }
}