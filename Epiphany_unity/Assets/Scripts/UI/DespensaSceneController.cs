using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DespensaSceneController : MonoBehaviour
{
    [Header("Gerenciamento de Estado")]
    [SerializeField] private string completionFlag = "DespensaSceneCompleted";

    [Header("Referências da Cena")]
    [SerializeField] private GameObject vethObject;
    [SerializeField] private NPCTourGuide vethTourGuide;
    // A referência ao FadeController foi removida, pois não é mais necessária.

    [Header("Sequência de Diálogos")]
    [SerializeField] private DialogueData dialogoEncontro;
    [SerializeField] private DialogueData dialogoEspirito;
    [SerializeField] private DialogueData dialogoReflexao;
    [SerializeField] private DialogueData dialogoCupcakes;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            if (vethObject != null) vethObject.SetActive(false);
            this.enabled = false;
            return;
        }
    }

    // --- LÓGICA DE CORRIDA CORRIGIDA ---
    private IEnumerator Start()
    {
        // Espera um frame para garantir que Veth seja renderizado na posição inicial.
        yield return null;

        // Inicia a primeira caminhada de Veth no frame seguinte.
        if (vethTourGuide != null)
        {
            vethTourGuide.StartTour();
        }
    }
    // --- FIM DA CORREÇÃO ---

    public void StartDialogueSequence()
    {
        StartCoroutine(FullDialogueRoutine());
    }

    private IEnumerator FullDialogueRoutine()
    {
        if (vethTourGuide != null) vethTourGuide.enabled = false;

        // Diálogo 1
        DialogueManager.Instance.StartDialogue(dialogoEncontro);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        yield return new WaitForSeconds(0.5f);

        // --- EFEITO DE PISCAR REMOVIDO ---

        // Diálogo 2
        DialogueManager.Instance.StartDialogue(dialogoEspirito);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        yield return new WaitForSeconds(0.5f);

        // Diálogo 3
        DialogueManager.Instance.StartDialogue(dialogoReflexao);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        yield return new WaitForSeconds(0.5f);

        // Diálogo 4 (Final)
        DialogueManager.Instance.StartDialogue(dialogoCupcakes);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());

        if (vethTourGuide != null)
        {
            vethTourGuide.OnTourCompleted.AddListener(HandleVethExit);
            vethTourGuide.enabled = true;
        }
    }

    // --- CORROTINA BLINKEFFECT REMOVIDA ---

    private void HandleVethExit()
    {
        if (vethObject != null) vethObject.SetActive(false);
        
        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
    }
}