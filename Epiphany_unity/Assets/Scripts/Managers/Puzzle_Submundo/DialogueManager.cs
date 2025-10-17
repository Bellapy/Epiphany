using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public static event System.Action OnDialogueEnd;

    private GameObject dialogueBox;
    private CanvasGroup dialogueBoxCanvasGroup; // Nova variável
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
    private bool isDialogueAutomatic = false; 
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
        dialogueBoxCanvasGroup = null; // Limpa a referência
        speakerNameText = null;
        dialogueText = null;
        speakerPortrait = null;
        choiceButtons = null;
    }

    public void ConnectUI(UIDialogueConnector connector)
    {
        dialogueBox = connector.dialogueBox;
        if (dialogueBox != null)
        {
            dialogueBoxCanvasGroup = dialogueBox.GetComponent<CanvasGroup>();
            if (dialogueBoxCanvasGroup == null)
            {
                dialogueBoxCanvasGroup = dialogueBox.AddComponent<CanvasGroup>();
            }
        }
        speakerNameText = connector.speakerNameText;
        dialogueText = connector.dialogueText;
        speakerPortrait = connector.speakerPortrait;
        choiceButtons = connector.choiceButtons;
        if (dialogueBox != null) dialogueBox.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogue, bool isAutomatic = false)
{
        StopAllCoroutines();
        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 1f;

        if (dialogueBox == null) { Debug.LogError("ERRO: O Dialogue Box não foi conectado nesta cena!"); return; }

        isDialogueAutomatic = isAutomatic; // <<< ADICIONE ESTA LINHA para guardar o estado

        LastChoiceIndex = -1;
        currentDialogueData = dialogue;
        dialogueBox.SetActive(true);
        if (choiceButtons != null) foreach (Button button in choiceButtons) { if(button != null) button.gameObject.SetActive(false); }
        if (speakerNameText != null) { speakerNameText.gameObject.SetActive(true); speakerNameText.text = dialogue.speakerName; }
        if (speakerPortrait != null) {
            if (dialogue.speakerPortrait != null) { speakerPortrait.gameObject.SetActive(true); speakerPortrait.sprite = dialogue.speakerPortrait; speakerPortrait.enabled = true; }
            else { speakerPortrait.gameObject.SetActive(false); }
        }
        lines.Clear();
        foreach (DialogueLine line in dialogue.dialogueLines) { lines.Enqueue(line); }
        DisplayNextSentence();
    }

    public void StartReflection(ReflectionData reflection)
    {
        StopAllCoroutines();
        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 1f; // Garante visibilidade

        if (dialogueBox == null) { Debug.LogError("ERRO: O Dialogue Box não foi conectado nesta cena!"); return; }
        LastChoiceIndex = -1;
        currentDialogueData = null;
        dialogueBox.SetActive(true);
        if (speakerNameText != null) speakerNameText.gameObject.SetActive(false);
        if (speakerPortrait != null) speakerPortrait.gameObject.SetActive(false);
        if (choiceButtons != null) foreach (var button in choiceButtons) { if(button != null) button.gameObject.SetActive(false); }
        lines.Clear();
        Queue<DialogueLine> reflectionLines = new Queue<DialogueLine>();
        foreach (string sentence in reflection.reflectionLines) {
            reflectionLines.Enqueue(new DialogueLine { sentence = sentence });
        }
        this.lines = reflectionLines;
        DisplayNextSentence();
    }

    public void StartReflectionWithFadeOut(ReflectionData reflection, float displayTime, float fadeTime)
    {
        StartReflection(reflection);
        StartCoroutine(FadeOutDialogueBox(displayTime, fadeTime));
    }

    private IEnumerator FadeOutDialogueBox(float waitTime, float fadeDuration)
    {
        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(waitTime);
        if (dialogueBoxCanvasGroup != null) {
            float startAlpha = dialogueBoxCanvasGroup.alpha;
            float timer = 0f;
            while (timer < fadeDuration) {
                timer += Time.deltaTime;
                dialogueBoxCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
                yield return null;
            }
            dialogueBoxCanvasGroup.alpha = 0f;
        }
        CloseDialogueBox();
    }

    public void Update()
{
    if (dialogueBox == null || !dialogueBox.activeInHierarchy || isDialogueAutomatic) { return; } // <<< ADICIONE "|| isDialogueAutomatic"
    if (isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) {
            StopAllCoroutines();
            if (dialogueText != null) dialogueText.text = currentFullSentence;
            isTyping = false;
        } else if (!isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) {
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

    // --- NOVA LÓGICA AUTOMÁTICA ---
    if (isDialogueAutomatic)
    {
        // Se for automático, inicia a coroutine para avançar sozinho
        StartCoroutine(AutoAdvanceAfterDelay(2.0f)); // Espera 2 segundos
    }
    // --- FIM DA NOVA LÓGICA ---
}
    private IEnumerator AutoAdvanceAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    DisplayNextSentence();
}

    private void EndDialogue()
    {
        if (currentDialogueData != null && currentDialogueData.hasChoice) {
            if (dialogueText != null) {
                dialogueText.text = currentDialogueData.choicePrompt;
            }
            PresentChoice(currentDialogueData);
        } else {
            if(dialogueBox != null) dialogueBox.SetActive(false);
            OnDialogueEnd?.Invoke();
        }
    }

    private void PresentChoice(DialogueData data)
    {
        if (choiceButtons == null || choiceButtons.Count == 0) return;
        for (int i = 0; i < data.choiceOptions.Count; i++) {
            if (i < choiceButtons.Count) {
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
    // Esconde os botões imediatamente
    foreach (Button button in choiceButtons) { if(button != null) button.gameObject.SetActive(false); }

    // Pega a opção que foi escolhida
    ChoiceOption chosenOption = currentDialogueData.choiceOptions[choiceIndex];

    // Se a opção escolhida tem um próximo diálogo, comece-o.
    if (chosenOption.nextDialogue != null)
    {
        StartDialogue(chosenOption.nextDialogue);
    }
    // Senão (como no nosso botão "Sair"), apenas feche a caixa de diálogo.
    else
    {
        CloseDialogueBox();
        OnDialogueEnd?.Invoke(); // Dispara o evento de fim de diálogo para que outros sistemas (se houver) possam reagir.
    }
}

    public bool IsDialogueBoxActive()
    {
        if (dialogueBox == null) return false;
        return dialogueBox.activeInHierarchy;
    }

    public void CloseDialogueBox()
    {
        StopAllCoroutines();
        isTyping = false;
        if (dialogueBox != null) {
            dialogueBox.SetActive(false);
        }
    }
}