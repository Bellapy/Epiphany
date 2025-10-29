using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZricSceneController : MonoBehaviour
{
    [Header("Gerenciamento de Estado da Cena")]
    [Tooltip("Nome da flag que marca esta cena como concluída.")]
    [SerializeField] private string completionFlag = "ZricSceneCompleted";
    [Tooltip("Arraste o GameObject da Ayla aqui.")]
    [SerializeField] private GameObject aylaObject;
    [Tooltip("Arraste o GameObject do Zric aqui.")]
    [SerializeField] private GameObject zricObject;

    [Header("Diálogos da Cena")]
    [SerializeField] private List<DialogueData> initialDialogueSequence;
    [SerializeField] private DialogueData postPuzzleDialogue;
    
    [Header("Sequência Pós-Puzzle")]
    [SerializeField] private ItemData fluteItemData;
    
    // --- LINHA ADICIONADA (A CORREÇÃO) ---
    [Tooltip("Arraste o ItemAcquired_Panel do Canvas local aqui.")]
    [SerializeField] private ItemAcquiredDisplay itemAcquiredDisplay;
    // --- FIM DA CORREÇÃO ---

    [SerializeField] private DialogueData aylaPosPuzzleDialogue;
    [SerializeField] private NPCTourGuide aylaTourGuide;
    [SerializeField] private GameObject transitionDoorTrigger;
    
    [Header("Configurações de Cena")]
    [SerializeField] private float delayToStartDialogue = 3.0f;
    
    private int currentDialogueIndex = 0;
    private bool isInitialSequenceComplete = false;
    private FadeController fadeController;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            if (aylaObject != null) aylaObject.SetActive(false);
            if (zricObject != null) zricObject.SetActive(false);
            if (transitionDoorTrigger != null) transitionDoorTrigger.SetActive(true);
            this.enabled = false;
            return;
        }
    }

    void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();
        if (transitionDoorTrigger != null) transitionDoorTrigger.SetActive(false);
        StartCoroutine(SceneStartSequence());
    }

    private void OnEnable() 
    { 
        if (DialogueManager.Instance != null) DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
    }

    private void OnDisable() 
    { 
        if (DialogueManager.Instance != null) DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
    }

    private IEnumerator SceneStartSequence()
    {
        if (fadeController != null)
        {
            fadeController.StartFadeIn(null, Color.white);
            yield return new WaitForSeconds(fadeController.fadeDuration);
        }
        yield return new WaitForSeconds(delayToStartDialogue);
        StartNextDialogueInSequence();
    }

    private void StartNextDialogueInSequence()
    {
        if (DialogueManager.Instance == null) return;
        if (currentDialogueIndex < initialDialogueSequence.Count)
        {
            DialogueManager.Instance.StartDialogue(initialDialogueSequence[currentDialogueIndex++]);
        }
        else
        {
            isInitialSequenceComplete = true;
            FindFirstObjectByType<PlayerController>()?.EnableMovement();
        }
    }

    private void HandleDialogueEnd()
    {
        if (!isInitialSequenceComplete) StartNextDialogueInSequence();
    }

    public void OnPuzzleSolved()
    {
        if (DialogueManager.Instance == null) return;
        DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        DialogueManager.Instance.StartDialogue(postPuzzleDialogue);
        DialogueManager.Instance.OnDialogueEnd += HandlePostPuzzleDialogueEnd;
    }

    private void HandlePostPuzzleDialogueEnd()
    {
        StartCoroutine(ItemAndAylaSequence());
    }

    private IEnumerator ItemAndAylaSequence()
    {
        if (GameManager.Instance != null) GameManager.Instance.PlayerHasFlute = true;

        if (itemAcquiredDisplay != null && fluteItemData != null)
        {
            itemAcquiredDisplay.ShowItem(fluteItemData);
        }

        // Espera o tempo total da animação do pop-up (0.5s fade in + 3s display + 0.5s fade out)
        yield return new WaitForSeconds(4.0f); 

        if (DialogueManager.Instance != null)
        {
            // Unsubscribe aqui é uma boa prática para evitar chamadas duplas
            DialogueManager.Instance.OnDialogueEnd -= HandlePostPuzzleDialogueEnd;
            DialogueManager.Instance.StartDialogue(aylaPosPuzzleDialogue);
            DialogueManager.Instance.OnDialogueEnd += HandleAylaDialogueEnd;
        }
    }

    private void HandleAylaDialogueEnd()
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.OnDialogueEnd -= HandleAylaDialogueEnd;
        
        if (aylaTourGuide != null) aylaTourGuide.StartTour();
        if (transitionDoorTrigger != null) transitionDoorTrigger.SetActive(true);

        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
    }
}