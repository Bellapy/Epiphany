using UnityEngine;
using System.Collections;

public class CozinhaSceneController : MonoBehaviour
{
    [Header("Referências de Atores e Roteiro")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private Transform aylaTransform;
    [SerializeField] private Transform aylaPontoDePartida;
    [SerializeField] private NPCTourGuide aylaTourGuide;
    [SerializeField] private SitController poltronaController;
    [SerializeField] private SceneTransitionTrigger portaDeSaida;

    [Header("Conteúdo da Conversa")]
    [Tooltip("O diálogo principal que começa quando o jogador se senta.")]
    [SerializeField] private DialogueData dialogoConversaPrincipal;

    private bool isConversationActive = false;

    void Start()
    {
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", true);
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
    
    public void IniciarConversaPrincipal()
    {
        if (isConversationActive || DialogueManager.Instance == null) return;

        isConversationActive = true;
        DialogueManager.Instance.StartDialogue(dialogoConversaPrincipal);
    }
    
    private void HandleDialogueEnd()
    {
        if (!isConversationActive) return;

        isConversationActive = false;
        StartCoroutine(SequenciaFinalAyla());
    }
    
    private IEnumerator SequenciaFinalAyla()
    {
        if (portaDeSaida != null)
        {
            portaDeSaida.Unlock();
        }

        if (poltronaController != null)
        {
            poltronaController.Levantar();
        }
        else
        {
            yield break;
        }

        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", false);
        
        yield return new WaitForSeconds(0.5f);

        if (aylaPontoDePartida != null && aylaTransform != null)
        {
            aylaTransform.position = aylaPontoDePartida.position;
        }
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.EnableMovement();
        }
        
        if (aylaTourGuide != null)
        {
            aylaTourGuide.StartTour();
        }
    }
}