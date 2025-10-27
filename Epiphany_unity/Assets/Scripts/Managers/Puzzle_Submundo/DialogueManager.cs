using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public event System.Action OnDialogueEnd;
    public event System.Action<int> OnDialogueLineStart;

    private GameObject dialogueBox;
    private CanvasGroup dialogueBoxCanvasGroup;
    private TextMeshProUGUI speakerNameText;
    private TextMeshProUGUI dialogueText;
    private Image speakerPortrait;
    private List<Button> choiceButtons;

    [Header("Configurações")]
    [SerializeField] private float typingSpeed = 0.04f;

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
    private bool isTyping = false;
    private string currentFullSentence;
    private DialogueData currentDialogueData;
    private bool isDialogueAutomatic = false; 
    public int LastChoiceIndex { get; private set; } = -1;

    // REMOVIDO: OnEnable, OnDisable, e OnSceneLoaded não são mais necessários.
    // A lógica de limpeza foi movida para ConnectUI para evitar race conditions.

    public void ConnectUI(UIDialogueConnector connector)
    {
        // --- LÓGICA DE LIMPEZA MOVIDA PARA CÁ ---
        // Garante que qualquer diálogo antigo seja interrompido antes de conectar a nova UI.
        StopAllCoroutines();
        isTyping = false;
        currentDialogueData = null;
        LastChoiceIndex = -1;
        // --- FIM DA LÓGICA DE LIMPEZA ---

        if (connector == null) return;

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
        if (dialogueBox == null)
        {
            Debug.LogError("[DialogueManager] Tentou iniciar um diálogo, mas nenhuma UI está conectada! Verifique se a cena tem um UIDialogueConnector.");
            return;
        }

        StopAllCoroutines();
        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 1f;

        isDialogueAutomatic = isAutomatic;

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
        if (dialogueBox == null)
        {
            Debug.LogError("[DialogueManager] Tentou iniciar uma reflexão, mas nenhuma UI está conectada!");
            return;
        }

        StopAllCoroutines();
        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 1f;

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
        if (dialogueBox == null || !dialogueBox.activeInHierarchy || isDialogueAutomatic) return;
        
        if (isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) 
        {
            StopAllCoroutines();
            if (dialogueText != null) dialogueText.text = currentFullSentence;
            isTyping = false;
        } 
        else if (!isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) 
        {
            DisplayNextSentence();
        }
    }

    private void DisplayNextSentence()
    {
        if (lines.Count == 0) { EndDialogue(); return; }

        DialogueLine currentLine = lines.Dequeue();
        
        if (currentDialogueData != null && currentDialogueData.dialogueLines != null)
        {
            int currentLineIndex = currentDialogueData.dialogueLines.IndexOf(currentLine);
            if (currentLineIndex != -1)
            {
                OnDialogueLineStart?.Invoke(currentLineIndex);
            }
        }

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

        if (isDialogueAutomatic)
        {
            StartCoroutine(AutoAdvanceAfterDelay(2.0f));
        }
    }
    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayNextSentence();
    }

    private void EndDialogue()
    {
        if (currentDialogueData != null && currentDialogueData.hasChoice) 
        {
            if (dialogueText != null) 
            {
                dialogueText.text = currentDialogueData.choicePrompt;
            }
            PresentChoice(currentDialogueData);
        } 
        else 
        {
            if(dialogueBox != null) dialogueBox.SetActive(false);
            OnDialogueEnd?.Invoke();
        }
    }

    private void PresentChoice(DialogueData data)
    {
        if (choiceButtons == null || choiceButtons.Count == 0) return;
        for (int i = 0; i < data.choiceOptions.Count; i++) {
            if (i < choiceButtons.Count && choiceButtons[i] != null) {
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

        ChoiceOption chosenOption = currentDialogueData.choiceOptions[choiceIndex];

        if (chosenOption.nextDialogue != null)
        {
            StartDialogue(chosenOption.nextDialogue);
        }
        else
        {
            CloseDialogueBox();
            OnDialogueEnd?.Invoke();
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
        if (dialogueBox != null) 
        {
            dialogueBox.SetActive(false);
        }
    }
}