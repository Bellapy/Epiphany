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

/*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// Awake is a special Unity method that is called when the script is loaded.
    /// We use it to connect the UI elements to the respective managers
    /// before any other code is executed.
    /// </summary>
/*******  8494f327-98c7-46d1-88e4-9887b5dff720  *******/    // A MUDANÇA É AQUI: Trocamos Awake por Start
    void Start() // Garante que a conexão aconteça depois que todos os managers (em Awake) já se inicializaram.
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