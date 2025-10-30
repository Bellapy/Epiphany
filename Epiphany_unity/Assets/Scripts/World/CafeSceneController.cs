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
    [Tooltip("O diálogo final de Veth que leva às escolhas.")]
    [SerializeField] private DialogueData vethChoiceIntroDialogue;

    private int currentDialogueIndex = 0;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            if (aylaObject != null) aylaObject.SetActive(false);
            if (vethObject != null) vethObject.SetActive(false);
            if (saidaBloqueio != null) saidaBloqueio.SetActive(false); // Libera a saída em visitas futuras
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
        
        yield return new WaitForSeconds(1.5f); // Pequeno atraso para a cena assentar

        DialogueManager.Instance.OnDialogueEnd += HandleLinearDialogueEnd;
        StartNextLinearDialogue();
    }

    private void StartNextLinearDialogue()
    {
        if (currentDialogueIndex < linearDialogues.Count)
        {
            DialogueManager.Instance.StartDialogue(linearDialogues[currentDialogueIndex]);
            
            if (linearDialogues[currentDialogueIndex].name == "Cafe_Veth_DespedidaAyla")
            {
                if (aylaTourGuide != null)
                {
                    // --- LÓGICA CORRIGIDA AQUI ---
                    // Adicionamos um listener para o evento de conclusão do tour de Ayla.
                    aylaTourGuide.OnTourCompleted.AddListener(HandleAylaTourCompletion);
                    aylaTourGuide.StartTour();
                    // --- FIM DA CORREÇÃO ---
                }
            }
            
            currentDialogueIndex++;
        }
        else
        {
            // Fim da sequência linear, começa a parte das escolhas
            DialogueManager.Instance.OnDialogueEnd -= HandleLinearDialogueEnd;
            DialogueManager.Instance.OnDialogueEnd += HandleChoiceDialogueEnd;
            DialogueManager.Instance.StartDialogue(vethChoiceIntroDialogue);
        }
    }

    private void HandleAylaTourCompletion()
    {
        // Esta função é chamada quando Ayla chega ao seu destino final (a porta).
        if (aylaObject != null)
        {
            aylaObject.SetActive(false); // Faz Ayla desaparecer.
        }
    }

    private void HandleLinearDialogueEnd()
    {
        StartNextLinearDialogue();
    }

    private void HandleChoiceDialogueEnd()
    {
        // Este evento é chamado após o último diálogo de escolha terminar.
        DialogueManager.Instance.OnDialogueEnd -= HandleChoiceDialogueEnd;
        
        // Inicia a saída de Veth
        if (vethTourGuide != null)
        {
            vethTourGuide.OnTourCompleted.AddListener(FinalizeScene);
            vethTourGuide.StartTour();
        }
    }

    private void FinalizeScene()
    {
        if (vethObject != null) vethObject.SetActive(false);
        if (saidaBloqueio != null) saidaBloqueio.SetActive(false); // Libera a saída
        
        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
        
        FindFirstObjectByType<PlayerController>()?.EnableMovement();
    }
}