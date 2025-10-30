using UnityEngine;
using System.Collections; // Importante: Adicionar para usar Corrotinas

public class TardigradeSceneController : MonoBehaviour
{
    [Header("Gerenciamento de Estado")]
    [SerializeField] private string completionFlag = "TardigradeSceneCompleted";

    [Header("Referências da Cena")]
    [SerializeField] private GameObject aylaObject;
    [SerializeField] private GameObject tardigradeObject;
    [SerializeField] private NPCTourGuide aylaTourGuide;
    [SerializeField] private DialogueData aylaDialogue;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            if (aylaObject != null) aylaObject.SetActive(false);
            if (tardigradeObject != null) tardigradeObject.SetActive(false);
            this.enabled = false;
            return;
        }
    }

    // --- LÓGICA CORRIGIDA AQUI ---
    private IEnumerator Start()
    {
        // Espera por um único frame.
        // Isso garante que a cena seja renderizada uma vez com Ayla na sua posição inicial.
        yield return null; 

        // Agora, no frame seguinte, o tour é iniciado.
        if (aylaTourGuide != null)
        {
            aylaTourGuide.StartTour();
        }
    }
    // --- FIM DA CORREÇÃO ---

    public void TriggerAylaDialogue()
    {
        if (aylaTourGuide != null) aylaTourGuide.enabled = false;

        if (DialogueManager.Instance != null && aylaDialogue != null)
        {
            DialogueManager.Instance.OnDialogueEnd += HandleAylaDialogueEnd;
            DialogueManager.Instance.StartDialogue(aylaDialogue);
        }
    }

    private void HandleAylaDialogueEnd()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleAylaDialogueEnd;
        }

        if (aylaTourGuide != null) aylaTourGuide.enabled = true;

        aylaTourGuide.OnTourCompleted.AddListener(HandleTourCompletion);
    }

    private void HandleTourCompletion()
    {
        if (aylaObject != null) aylaObject.SetActive(false);
        
        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
    }
}