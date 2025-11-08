using UnityEngine;
using System.Collections;

public class TardigradeSceneController : MonoBehaviour
{
    [Header("Gerenciamento de Estado")]
    [SerializeField] private string completionFlag = "TardigradeSceneCompleted";

    [Header("Referências da Cena")]
    [SerializeField] private GameObject aylaObject;
    [SerializeField] private GameObject tardigradeObject;
    [SerializeField] private NPCTourGuide aylaTourGuide;
    // A referência ao DialogueData foi removida, pois não é mais necessária.

    private void Awake()
    {
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            if (aylaObject != null) aylaObject.SetActive(false);
            // Mantemos o tardígrado visível se o jogador retornar à cena.
            // if (tardigradeObject != null) tardigradeObject.SetActive(false); 
            this.enabled = false;
            return;
        }
    }

    private IEnumerator Start()
    {
        // Garante que o evento de conclusão esteja conectado desde o início.
        if (aylaTourGuide != null)
        {
            aylaTourGuide.OnTourCompleted.AddListener(HandleTourCompletion);
        }

        // Espera um frame para garantir que tudo na cena foi inicializado.
        yield return null; 

        // Inicia o tour da Ayla.
        if (aylaTourGuide != null)
        {
            aylaTourGuide.StartTour();
        }
    }

    // As funções TriggerAylaDialogue e HandleAylaDialogueEnd foram removidas.

    private void HandleTourCompletion()
    {
        // Quando Ayla termina o percurso, seu GameObject é desativado.
        if (aylaObject != null) aylaObject.SetActive(false);
        
        // Salva o estado para que a Ayla não apareça novamente nesta cena.
        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
    }
}