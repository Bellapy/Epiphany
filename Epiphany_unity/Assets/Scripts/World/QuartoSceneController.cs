using UnityEngine;
using System.Collections;

public class QuartoSceneController : MonoBehaviour
{
    [Header("Configurações da Cena")]
    [SerializeField] private DialogueData dialogoDaAyla;
    [SerializeField] private float atrasoParaDialogo = 2.0f;

    [Header("Referências da Ayla")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private Transform aylaTransform;
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;
    [SerializeField] private Transform pontoDestinoPorta;
    [SerializeField] private float velocidadeCaminhada = 1.0f;

    // <<< INÍCIO DA CORREÇÃO >>>


    void Start()
    {
        // Não fazemos mais nada no Start diretamente, chamamos a sequência.
        StartCoroutine(SceneSequence());
    }

    // OnEnable e OnDisable não são mais necessários para este script.

    private IEnumerator SceneSequence()
    {
        yield return new WaitForSeconds(atrasoParaDialogo);

        if (dialogoDaAyla != null && DialogueManager.Instance != null)
        {
            // 1. Inscreve-se no evento IMEDIATAMENTE ANTES de iniciar o diálogo.
            DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
            
            // 2. Inicia o diálogo.
            DialogueManager.Instance.StartDialogue(dialogoDaAyla);
        }
        else
        {
            Debug.LogError("[QuartoController] Não foi possível iniciar o diálogo. Verifique as referências.");
        }
    }

    private void HandleDialogueEnd()
    {
        // 3. A primeira coisa a fazer é se desinscrever para não receber eventos futuros.
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }

        // 4. Agora, executa a lógica da cena.
        StartCoroutine(CaminhadaAtePortaCoroutine());
    }

    private IEnumerator CaminhadaAtePortaCoroutine()
    {
        if (pontoDestinoPorta == null || aylaTransform == null || aylaAnimator == null) 
        {
            Debug.LogError("[QuartoController] ERRO: Uma ou mais referências para a caminhada da Ayla estão NULAS!");
            yield break;
        }

        Vector2 direcao = (pontoDestinoPorta.position - aylaTransform.position).normalized;
        
        aylaAnimator.SetInteger("MovementState", 5);
        if (aylaSpriteRenderer != null)
        {
            aylaSpriteRenderer.flipX = direcao.x < 0;
        }

        while (Vector3.Distance(aylaTransform.position, pontoDestinoPorta.position) > 0.1f)
        {
            aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestinoPorta.position, velocidadeCaminhada * Time.deltaTime);
            yield return null;
        }

        aylaTransform.position = pontoDestinoPorta.position;
        aylaAnimator.SetInteger("MovementState", 4);
        
        aylaTransform.gameObject.SetActive(false);
    }
    
    // Boa prática: garantir a limpeza caso o objeto seja destruído no meio do processo.
    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }
    }
    // <<< FIM DA CORREÇÃO >>>
}