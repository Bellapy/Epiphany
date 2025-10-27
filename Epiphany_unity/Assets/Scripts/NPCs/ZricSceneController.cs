using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZricSceneController : MonoBehaviour
{
    [Header("Diálogos da Cena")]
    [SerializeField] private List<DialogueData> initialDialogueSequence;
    [SerializeField] private DialogueData postPuzzleDialogue; // Diálogo do Zric
    
    // --- NOVAS LINHAS ADICIONADAS ---
    [Header("Sequência Pós-Puzzle de Ayla")]
    [Tooltip("O diálogo que Ayla fala após Zric.")]
    [SerializeField] private DialogueData aylaPosPuzzleDialogue;
    [Tooltip("Referência ao componente NPCTourGuide no GameObject da Ayla.")]
    [SerializeField] private NPCTourGuide aylaTourGuide;
    [Tooltip("O GameObject que serve como gatilho da porta de transição.")]
    [SerializeField] private GameObject transitionDoorTrigger;
    // --- FIM DAS NOVAS LINHAS ---

    [Header("Configurações de Cena")]
    [SerializeField] private float delayToStartDialogue = 3.0f;
    
    // Removido: A transição agora é física, não mais controlada por este script.
    // [Header("Configuração de Transição de Saída")]
    // [SerializeField] private string nextSceneName = "Lojinha";
    // [SerializeField] private string spawnPointInNextScene = "SpawnFromZric";

    private int currentDialogueIndex = 0;
    private bool isInitialSequenceComplete = false;
    private FadeController fadeController;

    void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();
        
        // --- NOVA LINHA ADICIONADA ---
        // Garante que a porta comece desativada.
        if (transitionDoorTrigger != null)
        {
            transitionDoorTrigger.SetActive(false);
        }
        // --- FIM DA NOVA LINHA ---

        StartCoroutine(SceneStartSequence());
    }

    private void OnEnable() 
    { 
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
        }
    }

    private void OnDisable() 
    { 
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }
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
            DialogueManager.Instance.StartDialogue(initialDialogueSequence[currentDialogueIndex]);
            currentDialogueIndex++;
        }
        else
        {
            isInitialSequenceComplete = true;
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.EnableMovement();
            }
        }
    }

    private void HandleDialogueEnd()
    {
        if (!isInitialSequenceComplete)
        {
            StartNextDialogueInSequence();
        }
    }

    public void OnPuzzleSolved()
    {
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        DialogueManager.Instance.StartDialogue(postPuzzleDialogue);
        DialogueManager.Instance.OnDialogueEnd += HandlePostPuzzleDialogueEnd;
    }

    // --- LÓGICA COMPLETAMENTE ALTERADA ---
    private void HandlePostPuzzleDialogueEnd()
    {
        // Esta função é chamada quando o diálogo do ZRIC termina.
        if (DialogueManager.Instance == null) return;

        // Agora, em vez de terminar a cena, iniciamos o diálogo da AYLA.
        DialogueManager.Instance.OnDialogueEnd -= HandlePostPuzzleDialogueEnd;
        DialogueManager.Instance.StartDialogue(aylaPosPuzzleDialogue);
        DialogueManager.Instance.OnDialogueEnd += HandleAylaDialogueEnd;
    }

    // --- NOVA FUNÇÃO ADICIONADA ---
    private void HandleAylaDialogueEnd()
    {
        // Esta função é chamada quando o diálogo da AYLA termina.
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleAylaDialogueEnd;
        }

        // Inicia a caminhada da Ayla.
        if (aylaTourGuide != null)
        {
            aylaTourGuide.StartTour();
        }

        // Ativa a porta para que o jogador possa usá-la.
        if (transitionDoorTrigger != null)
        {
            transitionDoorTrigger.SetActive(true);
        }
    }
    // --- FIM DA NOVA FUNÇÃO ---
}