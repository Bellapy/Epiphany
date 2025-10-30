using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CafeSceneController : MonoBehaviour
{
    [Header("Gerenciamento de Estado")]
    [SerializeField] private string completionFlag = "CafeSceneCompleted";

    [Header("Referências de Atores")]
    [SerializeField] private GameObject aylaObject;
    [SerializeField] private GameObject vethObject;
    [SerializeField] private NPCTourGuide aylaTourGuide;
    [SerializeField] private NPCTourGuide vethTourGuide;
    [SerializeField] private GameObject saidaBloqueio;

    [Header("Sequência de Diálogos")]
    [Tooltip("Arraste todos os diálogos lineares na ordem correta.")]
    [SerializeField] private List<DialogueData> linearDialogues;
    [Tooltip("O diálogo que contém a PRIMEIRA pergunta para Veth.")]
    [SerializeField] private DialogueData vethFirstChoiceDialogue;

    private int currentDialogueIndex = 0;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            if (aylaObject != null) aylaObject.SetActive(false);
            if (vethObject != null) vethObject.SetActive(false);
            if (saidaBloqueio != null) saidaBloqueio.SetActive(false);
            this.enabled = false;
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(StartSceneSequence());
    }

    private IEnumerator StartSceneSequence()
    {
        // O movimento do jogador não é mais desabilitado aqui.
        yield return new WaitForSeconds(1.5f); // Pequeno atraso para a cena assentar

        DialogueManager.Instance.OnDialogueEnd += HandleLinearDialogueEnd;
        StartNextLinearDialogue();
    }

    private void StartNextLinearDialogue()
    {
        if (currentDialogueIndex < linearDialogues.Count)
        {
            DialogueManager.Instance.StartDialogue(linearDialogues[currentDialogueIndex]);
            
            // Gatilho para a saída de Ayla
            if (linearDialogues[currentDialogueIndex].name == "Cafe_Veth_DespedidaAyla")
            {
                if (aylaTourGuide != null)
                {
                    aylaTourGuide.OnTourCompleted.AddListener(HandleAylaTourCompletion);
                    aylaTourGuide.StartTour();
                }
            }
            
            currentDialogueIndex++;
        }
        else
        {
            // Fim da sequência linear, começa a parte das escolhas
            DialogueManager.Instance.OnDialogueEnd -= HandleLinearDialogueEnd;
            DialogueManager.Instance.OnDialogueEnd += HandleChoiceDialogueEnd;
            DialogueManager.Instance.StartDialogue(vethFirstChoiceDialogue);
        }
    }

    private void HandleLinearDialogueEnd()
    {
        StartNextLinearDialogue();
    }

    private void HandleAylaTourCompletion()
    {
        if (aylaObject != null)
        {
            aylaObject.SetActive(false);
        }
    }

    private void HandleChoiceDialogueEnd()
    {
        DialogueManager.Instance.OnDialogueEnd -= HandleChoiceDialogueEnd;
        
        if (vethTourGuide != null)
        {
            vethTourGuide.OnTourCompleted.AddListener(FinalizeScene);
            vethTourGuide.StartTour();
        }
    }

    private void FinalizeScene()
    {
        if (vethObject != null) vethObject.SetActive(false);
        if (saidaBloqueio != null) saidaBloqueio.SetActive(false);
        
        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
        
        // Não precisamos reabilitar o movimento do jogador, pois ele nunca foi desabilitado.
    }
}