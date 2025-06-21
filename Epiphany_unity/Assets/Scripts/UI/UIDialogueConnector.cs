// Em _Scripts/UI/UIDialogueConnector.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDialogueConnector : MonoBehaviour
{
    [Header("Referências da UI de Diálogo Nesta Cena")]
    public GameObject borderPanel;         // <<< A VARIÁVEL QUE FALTAVA
    public TextMeshProUGUI reflectionText;
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
            Debug.LogError("UIDialogueConnector não conseguiu encontrar uma instância do UIManager!");
        }
    }
}