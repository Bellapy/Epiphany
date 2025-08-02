using UnityEngine;
using System.Collections;

public class PuzzleSceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [SerializeField] private SpiritNPC crystalNpc;
    [SerializeField] private CrystalPuzzleManager puzzleManager;
    [SerializeField] private PlayerController player;

    private enum SceneState { Idle, ShowingIntro, AwaitingChoice, Done }
    private SceneState currentState = SceneState.Idle;

    private void OnEnable()
    {
        DialogueManager.OnDialogueEnd += HandleDialogueEnd;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnd -= HandleDialogueEnd;
    }

    public void BeginSceneSequence()
    {
        if (currentState != SceneState.Idle) return;
        
        if (crystalNpc == null || puzzleManager == null || player == null)
        {
            Debug.LogError("[PuzzleSceneController] ERRO CRÍTICO: Referências não configuradas!");
            return;
        }
        
        currentState = SceneState.ShowingIntro;
        player.DisableMovement();
        if (AudioManager.Instance != null) AudioManager.Instance.StopMusicWithFade(1.5f);
        DialogueManager.Instance.StartDialogue(crystalNpc.introDialogue);
    }

    private void HandleDialogueEnd()
    {
        if (currentState != SceneState.ShowingIntro && currentState != SceneState.AwaitingChoice)
        {
            return;
        }

        if (currentState == SceneState.ShowingIntro)
        {
            currentState = SceneState.AwaitingChoice;
            StartCoroutine(PlayMelodyAndShowChoice());
        }
        else if (currentState == SceneState.AwaitingChoice)
        {
            // Para TODAS as corrotinas neste script (matando qualquer zumbi)
            StopAllCoroutines();
            
            if (DialogueManager.Instance.LastChoiceIndex == 0) // "Sim"
            {
                currentState = SceneState.AwaitingChoice; 
                StartCoroutine(PlayMelodyAndShowChoice());
            }
            else // "Não"
            {
                currentState = SceneState.Done;
                if (puzzleManager != null)
                {
                    puzzleManager.StopMelodyPlayback();
                }
                puzzleManager.ActivatePuzzle();
            }
        }
    }
    
    private IEnumerator PlayMelodyAndShowChoice()
    {
        if (puzzleManager != null)
        {
            puzzleManager.StopMelodyPlayback();
        }
        
        puzzleManager.PlaySolutionSequence();
        
        yield return new WaitForSeconds(puzzleManager.GetSequenceDuration() + 0.2f);
        
        // Verificação de segurança: só mostra o diálogo se ainda estivermos esperando uma escolha
        if(currentState == SceneState.AwaitingChoice)
        {
            DialogueManager.Instance.StartDialogue(crystalNpc.postMelodyAndChoiceDialogue);
        }
    }
}