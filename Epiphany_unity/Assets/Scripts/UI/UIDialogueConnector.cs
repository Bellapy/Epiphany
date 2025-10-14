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
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ConnectUI(this);
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ConnectUI(this);
        }
    }
}