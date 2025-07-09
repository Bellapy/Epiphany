using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System; // ADICIONAMOS ESTA LINHA DE VOLTA

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    #region Variáveis de UI
    [Header("Componentes da UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerPortrait;

    [Header("Configurações do Texto")]
    [SerializeField] private float typingSpeed = 0.04f;
    #endregion

    #region Variáveis de Controle
    private Queue<string> sentences;
    private bool isTyping = false;
    private string currentFullSentence;
    private Action onDialogueCompleteCallback; // VARIÁVEL PARA GUARDAR A AÇÃO FINAL
    #endregion

    private void Start()
    {
        sentences = new Queue<string>();
        dialogueBox.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueBox.activeInHierarchy) return;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    // A FUNÇÃO AGORA ACEITA A "AÇÃO" COMO UM PARÂMETRO
    public void StartDialogue(DialogueData dialogue, Action onComplete = null)
    {
        dialogueBox.SetActive(true);
        this.onDialogueCompleteCallback = onComplete; // Guarda a ação para usar depois

        speakerNameText.text = dialogue.speakerName;
        if (dialogue.speakerPortrait != null)
        {
            speakerPortrait.sprite = dialogue.speakerPortrait;
            speakerPortrait.enabled = true;
        }
        else
        {
            speakerPortrait.enabled = false;
        }

        sentences.Clear();
        foreach (string sentence in dialogue.dialogueLines)
        {
            sentences.Enqueue(sentence);
        }

        AdvanceDialogue();
    }
    
    public void AdvanceDialogue()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentFullSentence;
            isTyping = false;
        }
        else
        {
            DisplayNextSentence();
        }
    }

    private void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentFullSentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentFullSentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);

        // AQUI ESTÁ A "LIGAÇÃO DE VOLTA"
        if (onDialogueCompleteCallback != null)
        {
            onDialogueCompleteCallback.Invoke();
            onDialogueCompleteCallback = null;
        }
    }
}