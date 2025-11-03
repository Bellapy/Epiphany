using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDialogueConnector : MonoBehaviour
{
    [Header("Componentes de Diálogo e Reflexão")]
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image speakerPortrait;
    public List<Button> choiceButtons;
    
    [Header("Componentes de Prompt de Interação")]
    public CanvasGroup interactionPromptCanvasGroup;

    void Start() 
    {
        Debug.Log("<color=blue>[UIDialogueConnector.Start] Vou tentar me conectar aos managers agora.</color>");

        // Conecta com o DialogueManager
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ConnectUI(this);
        }
        else
        {
            Debug.LogError("[UIDialogueConnector] Não foi possível encontrar o DialogueManager.Instance!");
        }

        // Conecta com o UIManager
        if (UIManager.Instance != null)
        {
            Debug.Log("<color=cyan>...UIManager.Instance foi encontrado! Tentando conectar...</color>");
            UIManager.Instance.ConnectUI(this);
        }
        else
        {
            Debug.LogError("[UIDialogueConnector] Não foi possível encontrar o UIManager.Instance! Verifique a Script Execution Order.");
        }
    }
}