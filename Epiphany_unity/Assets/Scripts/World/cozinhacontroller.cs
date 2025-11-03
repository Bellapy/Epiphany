using UnityEngine;
using System.Collections;

public class CozinhaSceneController : MonoBehaviour
{
    [Header("Referências de Atores e Roteiro")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private Transform aylaTransform;
    [SerializeField] private Transform aylaPontoDePartida;
    [SerializeField] private SitController poltronaController; // O nome no seu Inspector é SitController
    [SerializeField] private SceneTransitionTrigger portaDeSaida;
    
    // <<< CORREÇÃO: A referência ao Ayla Tour Guide foi removida, pois não é mais necessária >>>
    // [SerializeField] private NPCTourGuide aylaTourGuide;

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
    
    // <<< CORREÇÃO APLICADA NESTA CORROTINA >>>
    private IEnumerator SequenciaFinalAyla()
    {
        // 1. Destranca a porta de saída
        if (portaDeSaida != null)
        {
            portaDeSaida.Unlock();
        }

        // 2. Levanta o jogador da poltrona
        if (poltronaController != null)
        {
            poltronaController.Levantar();
        }
        
        // 3. Ayla se levanta
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", false);
        
        // Pequena pausa para a animação de levantar acontecer
        yield return new WaitForSeconds(0.75f);

        // 4. Ayla se teleporta para o ponto de partida
        if (aylaPontoDePartida != null && aylaTransform != null)
        {
            aylaTransform.position = aylaPontoDePartida.position;
        }
        
        // 5. Devolve o controle ao jogador
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.EnableMovement();
        }
        
        // 6. Lógica de caminhada manual
        if (portaDeSaida != null && aylaTransform != null && aylaAnimator != null)
        {
            Transform pontoDestino = portaDeSaida.transform;
            SpriteRenderer aylaSpriteRenderer = aylaTransform.GetComponent<SpriteRenderer>();
            float velocidadeCaminhada = 1.2f; // Podemos ajustar essa velocidade

            // Ativa a animação de caminhada
            aylaAnimator.SetInteger("MovementState", 5);
            if (aylaSpriteRenderer != null)
            {
                aylaSpriteRenderer.flipX = (pontoDestino.position.x < aylaTransform.position.x);
            }

            // Move a Ayla até o destino
            while (Vector3.Distance(aylaTransform.position, pontoDestino.position) > 0.2f)
            {
                aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestino.position, velocidadeCaminhada * Time.deltaTime);
                yield return null;
            }

            // Desativa Ayla ao chegar
            aylaTransform.gameObject.SetActive(false);
        }
    }
}