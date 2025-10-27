using UnityEngine;
using System.Collections;

public class QuartoSceneController : MonoBehaviour
{
    [Header("Configurações da Cena")]
    [Tooltip("O diálogo que a Ayla vai falar nesta cena.")]
    [SerializeField] private DialogueData dialogoDaAyla;
    
    [Tooltip("Tempo em segundos que o jogo espera antes de iniciar o diálogo.")]
    [SerializeField] private float atrasoParaDialogo = 8.0f;

    [Header("Referências da Ayla")]
    [Tooltip("O Animator da Ayla nesta cena.")]
    [SerializeField] private Animator aylaAnimator;

    [Tooltip("O Transform da Ayla, para podermos movê-la.")]
    [SerializeField] private Transform aylaTransform;
    
    [Tooltip("O SpriteRenderer da Ayla, para podermos inverter a direção.")]
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;

    [Tooltip("O ponto final para onde a Ayla vai andar (a porta).")]
    [SerializeField] private Transform pontoDestinoPorta;

    [Tooltip("A velocidade com que a Ayla anda.")]
    [SerializeField] private float velocidadeCaminhada = 1.0f;

    void Start()
    {
        StartCoroutine(IniciarDialogoAposAtraso());
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

    private IEnumerator IniciarDialogoAposAtraso()
    {
        yield return new WaitForSeconds(atrasoParaDialogo);

        if (dialogoDaAyla != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogoDaAyla);
        }
    }

    private void HandleDialogueEnd()
    {
        StartCoroutine(CaminhadaAtePortaCoroutine());
    }

    private IEnumerator CaminhadaAtePortaCoroutine()
    {
        if (pontoDestinoPorta == null || aylaTransform == null || aylaAnimator == null) yield break;

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
    }
}