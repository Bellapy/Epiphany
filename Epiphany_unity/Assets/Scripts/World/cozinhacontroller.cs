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

    // <<< NOVA REFERÊNCIA >>>
    [Tooltip("Arraste a porta de saída que leva ao corredor aqui.")]
    [SerializeField] private SceneTransitionTrigger portaDeSaida;

    [Header("Conteúdo da Conversa")]
    [Tooltip("O diálogo principal que começa quando o jogador se senta.")]
    [SerializeField] private DialogueData dialogoConversaPrincipal;

    private bool isConversationActive = false;

    private void OnEnable() 
    {
        DialogueManager.OnDialogueEnd += HandleDialogueEnd; 
    }

    private void OnDisable() 
    {
        DialogueManager.OnDialogueEnd -= HandleDialogueEnd; 
    }

    void Start()
    {
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", true);
    }
    
    public void IniciarConversaPrincipal()
    {
        if (isConversationActive) return;

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
        // <<< A NOVA LÓGICA ESTÁ AQUI >>>
        // 1. Destranca a porta imediatamente.
        if (portaDeSaida != null)
        {
            portaDeSaida.Unlock();
        }
        else
        {
            Debug.LogWarning("Referência à porta de saída não definida. Não foi possível destrancá-la.");
        }

        // 2. Comanda o jogador a se levantar.
        if (poltronaController != null)
        {
            poltronaController.Levantar();
        }
        else
        {
            Debug.LogError("Referência ao SitController (poltrona) não definida!");
            yield break;
        }

        // 3. Ayla executa a animação de levantar.
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", false);
        
        yield return new WaitForSeconds(0.5f);

        // 4. Teleporta Ayla para a posição inicial de caminhada.
        if (aylaPontoDePartida != null)
        {
            aylaTransform.position = aylaPontoDePartida.position;
        }
        
        // 5. Libera o controle do jogador.
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.EnableMovement();
        }
        
        // 6. Inicia o tour da Ayla.
        if (aylaTourGuide != null)
        {
            aylaTourGuide.StartTour();
        }
    }
}