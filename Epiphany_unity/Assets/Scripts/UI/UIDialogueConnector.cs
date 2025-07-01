// Em _Scripts/UI/UIDialogueConnector.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDialogueConnector : MonoBehaviour
{
    [Header("Referências de Prompts (Opcional)")]
public CanvasGroup interactionPromptCanvasGroup;
    [Header("Referências da UI de Diálogo Nesta Cena")]
    [Tooltip("O painel principal que será ativado/desativado.")]
    public GameObject backgroundPanel;

    [Tooltip("O objeto de texto que exibirá as frases.")]
    public TextMeshProUGUI reflectionText;

    [Header("Referências do Retrato (Deixe vazio se não usar)")]
    public Image portraitImage;
    public GameObject portraitContainer;
    public Sprite playerPortrait;

    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ConnectUI(this);
        }
        else
        {
            Debug.LogError("UIDialogueConnector: Instância do UIManager não encontrada! O jogo foi iniciado pela cena correta?");
        }
    }
}