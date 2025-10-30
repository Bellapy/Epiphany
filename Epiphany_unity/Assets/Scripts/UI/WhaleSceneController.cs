using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhaleSceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [SerializeField] private FluteMinigameController minigameController;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject playerOnWhaleObject;
    [SerializeField] private GameObject mountTriggerObject;
    [SerializeField] private FadeController fadeController;

    [Header("Diálogos")]
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private DialogueData successDialogue;
    [SerializeField] private DialogueData finalDialogue;

    [Header("Configuração do Minigame")]
    [SerializeField] private List<FluteNote> fluteSequence;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        mountTriggerObject.SetActive(false);
        playerOnWhaleObject.SetActive(false);
    }

    // Esta função é chamada por um TriggerZone quando o jogador entra
    public void StartEncounter()
    {
        playerController?.DisableMovement();
        DialogueManager.Instance.OnDialogueEnd += HandleIntroDialogueEnd;
        DialogueManager.Instance.StartDialogue(introDialogue);
    }

    private void HandleIntroDialogueEnd()
    {
        DialogueManager.Instance.OnDialogueEnd -= HandleIntroDialogueEnd;
        // Independentemente da escolha, o minigame começa
        minigameController.OnMinigameCompleted.AddListener(HandleMinigameSuccess);
        minigameController.StartMinigame(fluteSequence);
    }

    private void HandleMinigameSuccess()
    {
        minigameController.OnMinigameCompleted.RemoveListener(HandleMinigameSuccess);
        DialogueManager.Instance.OnDialogueEnd += HandleSuccessDialogueEnd;
        DialogueManager.Instance.StartDialogue(successDialogue);
    }

    private void HandleSuccessDialogueEnd()
    {
        DialogueManager.Instance.OnDialogueEnd -= HandleSuccessDialogueEnd;
        mountTriggerObject.SetActive(true);
        playerController?.EnableMovement();
    }

    // Esta função é chamada pelo InteractionRelay do mountTriggerObject
    public void MountWhale()
    {
        playerObject.SetActive(false);
        playerOnWhaleObject.SetActive(true);
        mountTriggerObject.SetActive(false);
        StartCoroutine(FinalSequence());
    }

    private IEnumerator FinalSequence()
    {
        DialogueManager.Instance.StartDialogue(finalDialogue);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        yield return new WaitForSeconds(1.0f);

        fadeController.StartFadeOut(() => {
            GameManager.Instance.LoadScene("oceano");
        });
    }
}