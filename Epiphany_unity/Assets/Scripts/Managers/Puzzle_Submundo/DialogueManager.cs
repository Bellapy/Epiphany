using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    // O evento que já tínhamos adicionado.
    public static event System.Action OnDialogueEnd;

    private GameObject dialogueBox;
    private TextMeshProUGUI speakerNameText;
    private TextMeshProUGUI dialogueText;
    private Image speakerPortrait;
    private List<Button> choiceButtons;

    [Header("Configurações")]
    [SerializeField] private float typingSpeed = 0.04f;
    

    private Queue<DialogueLine> lines;
    private bool isTyping = false;
    private string currentFullSentence;
    private DialogueData currentDialogueData;

    public int LastChoiceIndex { get; private set; } = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        lines = new Queue<DialogueLine>();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        isTyping = false;
        currentDialogueData = null;
        LastChoiceIndex = -1;
        dialogueBox = null;
        speakerNameText = null;
        dialogueText = null;
        speakerPortrait = null;
        choiceButtons = null;
    }

    public void ConnectUI(UIDialogueConnector connector)
    {
        dialogueBox = connector.dialogueBox;
        speakerNameText = connector.speakerNameText;
        dialogueText = connector.dialogueText;
        speakerPortrait = connector.speakerPortrait;
        choiceButtons = connector.choiceButtons;
        if (dialogueBox != null) dialogueBox.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogueBox == null) { Debug.LogError("ERRO: O Dialogue Box não foi conectado nesta cena!"); return; }
        
        LastChoiceIndex = -1;
        currentDialogueData = dialogue;

        dialogueBox.SetActive(true);
        if (choiceButtons != null) foreach (Button button in choiceButtons) { if(button != null) button.gameObject.SetActive(false); }
        
        if (speakerNameText != null) { speakerNameText.gameObject.SetActive(true); speakerNameText.text = dialogue.speakerName; }
        if (speakerPortrait != null)
        {
            if (dialogue.speakerPortrait != null) { speakerPortrait.gameObject.SetActive(true); speakerPortrait.sprite = dialogue.speakerPortrait; speakerPortrait.enabled = true; }
            else { speakerPortrait.gameObject.SetActive(false); }
        }

        lines.Clear();
        foreach (DialogueLine line in dialogue.dialogueLines) { lines.Enqueue(line); }
        DisplayNextSentence();
    }

    public void StartReflection(ReflectionData reflection)
    {
        if (dialogueBox == null) { Debug.LogError("ERRO: O Dialogue Box não foi conectado nesta cena!"); return; }

        LastChoiceIndex = -1;
        currentDialogueData = null;
        dialogueBox.SetActive(true);

        if (speakerNameText != null) speakerNameText.gameObject.SetActive(false);
        if (speakerPortrait != null) speakerPortrait.gameObject.SetActive(false);
        if (choiceButtons != null) foreach (var button in choiceButtons) { if(button != null) button.gameObject.SetActive(false); }

        lines.Clear();
        Queue<DialogueLine> reflectionLines = new Queue<DialogueLine>();
        foreach (string sentence in reflection.reflectionLines)
        {
            reflectionLines.Enqueue(new DialogueLine { sentence = sentence });
        }
        this.lines = reflectionLines;
        
        DisplayNextSentence();
    }

    public void Update()
{
    // Se a caixa não estiver ativa, não faça nada.
    if (dialogueBox == null || !dialogueBox.activeInHierarchy) { return; }

    // Se estiver digitando, o input do jogador pula a digitação.
    if (isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
    {
        StopAllCoroutines();
        if (dialogueText != null) dialogueText.text = currentFullSentence;
        isTyping = false;
    }
    // Se não estiver digitando, o input avança para a próxima frase.
    else if (!isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
    {
        DisplayNextSentence();
    }
}

    private void DisplayNextSentence()
    {
        if (lines.Count == 0) { EndDialogue(); return; }
        DialogueLine currentLine = lines.Dequeue();
        currentFullSentence = currentLine.sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentFullSentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText == null) { yield break; }
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
    // Se o diálogo tiver uma escolha...
    if (currentDialogueData != null && currentDialogueData.hasChoice)
    {
        // ...Nós exibimos a frase da escolha SEM o efeito de digitação.
        if (dialogueText != null)
        {
            dialogueText.text = currentDialogueData.choicePrompt;
        }

        // E então, imediatamente, apresentamos os botões.
        PresentChoice(currentDialogueData);
    }
    // Se for um diálogo normal sem escolha...
    else
    {
        // ...Simplesmente fechamos a caixa e avisamos que acabou.
        if(dialogueBox != null) dialogueBox.SetActive(false);
        OnDialogueEnd?.Invoke();
    }
}

    private void PresentChoice(DialogueData data)
    {
        if (choiceButtons == null || choiceButtons.Count == 0) return;
        
        for (int i = 0; i < data.choiceOptions.Count; i++)
        {
            if (i < choiceButtons.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = data.choiceOptions[i].optionText;
                choiceButtons[i].onClick.RemoveAllListeners();
                int choiceIndex = i;
                choiceButtons[i].onClick.AddListener(() => SelectChoice(choiceIndex));
            }
        }
    }

    public void SelectChoice(int choiceIndex)
    {
        LastChoiceIndex = choiceIndex;
        foreach (Button button in choiceButtons) { if(button != null) button.gameObject.SetActive(false); }
        if (dialogueBox != null) dialogueBox.SetActive(false);

        // <<< ALTERAÇÃO 2: DISPARO DO EVENTO AO FAZER UMA ESCOLHA >>>
        // Isso garante que o PuzzleSceneController seja notificado.
        OnDialogueEnd?.Invoke(); 
    }

    public bool IsDialogueBoxActive()
    {
        if (dialogueBox == null) return false;
        return dialogueBox.activeInHierarchy;
    }
}