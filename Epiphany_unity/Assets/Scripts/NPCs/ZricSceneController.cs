// Em ZricSceneController.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necessário para List<>

public class ZricSceneController : MonoBehaviour
{
    [Header("Diálogos da Cena")]
    // Mudamos de uma única referência para uma lista
    [SerializeField] private List<DialogueData> initialDialogueSequence;
    [SerializeField] private DialogueData postPuzzleDialogue;

    [Header("Configurações de Cena")]
    [SerializeField] private float delayToStartDialogue = 3.0f;

    private int currentDialogueIndex = 0;

    private void OnEnable() { DialogueManager.OnDialogueEnd += HandleDialogueEnd; }
    private void OnDisable() { DialogueManager.OnDialogueEnd -= HandleDialogueEnd; }

    void Start()
    {
        StartCoroutine(SceneStartSequence());
    }

    private IEnumerator SceneStartSequence()
    {
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeIn(null, Color.white);
            yield return new WaitForSeconds(FadeController.Instance.fadeDuration);
        }
        yield return new WaitForSeconds(delayToStartDialogue);
        
        // Inicia o primeiro diálogo da sequência
        StartNextDialogueInSequence();
    }

    private void StartNextDialogueInSequence()
{
    if (currentDialogueIndex < initialDialogueSequence.Count)
    {
        DialogueManager.Instance.StartDialogue(initialDialogueSequence[currentDialogueIndex]);
        currentDialogueIndex++;
    }
    else
    {
        Debug.Log("Sequência de diálogo inicial concluída. Devolvendo controle ao jogador.");
        
        // <<< ADIÇÃO COMEÇA AQUI >>>
        // Encontra o PlayerController na cena e reabilita seu movimento.
        // Isso também trocará o Action Map de volta para "Player".
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.EnableMovement();
        }
        else
        {
            Debug.LogError("[ZricSceneController] Não foi possível encontrar o PlayerController para reativar o movimento!");
        }
        // <<< ADIÇÃO TERMINA AQUI >>>
    }
}

    // O evento OnDialogueEnd agora serve para avançar na sequência
    private void HandleDialogueEnd()
    {
        // Garante que só estamos avançando durante a sequência inicial
        if (currentDialogueIndex <= initialDialogueSequence.Count)
        {
            StartNextDialogueInSequence();
        }
    }

    public void OnPuzzleSolved()
    {
        // Remove a inscrição do evento para não interferir com o diálogo final
        DialogueManager.OnDialogueEnd -= HandleDialogueEnd;
        Debug.Log("Puzzle resolvido! Iniciando diálogo final.");
        DialogueManager.Instance.StartDialogue(postPuzzleDialogue);
    }
}