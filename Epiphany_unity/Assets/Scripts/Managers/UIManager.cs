// Em _Scripts/Managers/UIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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


    // --- Variáveis Internas (Controladas pelo Código) ---
    private GameObject currentBorderPanel;
    private TextMeshProUGUI currentReflectionText;
    private Image currentPortraitImage;
    private GameObject currentPortraitContainer;
    private Sprite currentPlayerPortrait;
    
    private Coroutine activeDialogueCoroutine;
    private bool isPanelVisible = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void ConnectUI(UIDialogueConnector connector)
    {
        Debug.Log("UIManager conectado com a UI da cena atual.");
        currentBorderPanel = connector.borderPanel;
        currentReflectionText = connector.reflectionText;
        currentPortraitImage = connector.portraitImage;
        currentPortraitContainer = connector.portraitContainer;
        currentPlayerPortrait = connector.playerPortrait;

        if (currentBorderPanel != null)
        {
            currentBorderPanel.SetActive(false); // Garante que a UI comece desligada
            isPanelVisible = false;
        }
    }

    // --- Métodos Públicos para Iniciar Diálogos ---

    public void ShowEnvironmentalReflection(ReflectionData data)
    {
        if (currentPortraitContainer != null) currentPortraitContainer.SetActive(false);
        StartShowingText(data.reflectionLines);
    }

    public void ShowPersonalReflection(ReflectionData data)
    {
        if (currentPortraitImage != null && currentPlayerPortrait != null)
        {
            currentPortraitImage.sprite = currentPlayerPortrait;
            if (currentPortraitContainer != null) currentPortraitContainer.SetActive(true);
        }
        StartShowingText(data.reflectionLines);
    }

    // Método para forçar o painel a se esconder (chamado pelo HideReflectionTrigger)
    public void HideReflection()
    {
        if (isPanelVisible)
        {
            if (activeDialogueCoroutine != null) StopCoroutine(activeDialogueCoroutine);
            if (currentBorderPanel != null) currentBorderPanel.SetActive(false);
            isPanelVisible = false;
        }
    }

    // --- Lógica Interna ---

    private void StartShowingText(List<string> lines)
    {
        if (currentBorderPanel == null)
        {
            Debug.LogWarning("UIManager tentou mostrar texto, mas nenhuma UI está conectada.");
            return;
        }
        
        // Se uma corrotina já estiver ativa, para ela antes de começar uma nova.
        if (activeDialogueCoroutine != null) StopCoroutine(activeDialogueCoroutine);
        activeDialogueCoroutine = StartCoroutine(DialogueRoutine(lines));
    }

    private IEnumerator DialogueRoutine(List<string> lines)
    {
        // 1. Mostra o painel
        currentBorderPanel.SetActive(true);
        isPanelVisible = true;

        // 2. Digita todas as frases
        for (int i = 0; i < lines.Count; i++)
        {
            yield return StartCoroutine(TypeSentence(lines[i]));
            if (i < lines.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenLines); 
            }
        }
        
        // 3. Espera um tempo na tela
        yield return new WaitForSeconds(timeOnScreenAfterTyping);
        
        // 4. Esconde o painel
        currentBorderPanel.SetActive(false);
        isPanelVisible = false;
        activeDialogueCoroutine = null;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        if (currentReflectionText == null) yield break;
        currentReflectionText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            currentReflectionText.text += letter;
            // Futuramente: Tocar som de blip aqui
            yield return new WaitForSeconds(timePerCharacter); 
        }
    }
}