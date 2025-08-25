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

    // --- Gerenciamento de Eventos ---

    private void OnEnable()
    {
        DialogueManager.OnDialogueEnd += HandleDialogueEnd;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnd -= HandleDialogueEnd;
    }

    // --- Lógica da Cena ---

    void Start()
    {
        StartCoroutine(IniciarDialogoAposAtraso());
    }

    private IEnumerator IniciarDialogoAposAtraso()
    {
        Debug.Log($"Cena do quarto iniciada. Aguardando {atrasoParaDialogo} segundos...");
        yield return new WaitForSeconds(atrasoParaDialogo);

        Debug.Log("Atraso finalizado. Iniciando diálogo da Ayla no quarto.");
        if (dialogoDaAyla != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogoDaAyla);
        }
    }

    // Este método é chamado automaticamente quando o diálogo termina
    private void HandleDialogueEnd()
    {
        Debug.Log("O diálogo da Ayla no quarto terminou! Iniciando a caminhada até a porta.");
        StartCoroutine(CaminhadaAtePortaCoroutine());
    }

    // Corrotina que controla a caminhada da Ayla
    private IEnumerator CaminhadaAtePortaCoroutine()
    {
        // 1. Calcula a direção e define a animação de caminhada
        Vector2 direcao = (pontoDestinoPorta.position - aylaTransform.position).normalized;
        
        // Define a animação usando os estados do Animator
        // Assumindo que a Ayla vai andar para o lado (estado 5)
        aylaAnimator.SetInteger("MovementState", 5); 
        // Inverte o sprite se a porta estiver à esquerda dela
        aylaSpriteRenderer.flipX = direcao.x < 0;

        // 2. Move a Ayla até o destino
        while (Vector3.Distance(aylaTransform.position, pontoDestinoPorta.position) > 0.1f)
        {
            aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestinoPorta.position, velocidadeCaminhada * Time.deltaTime);
            yield return null; // Espera o próximo frame
        }

        // 3. Chegou! Para a Ayla e a deixa parada de lado (estado 4)
        aylaTransform.position = pontoDestinoPorta.position;
        aylaAnimator.SetInteger("MovementState", 4);
        
        // Neste ponto, você poderia adicionar um fade out e a transição para a próxima cena.
        // Ex: FadeController.Instance.StartFadeOut(() => GameManager.Instance.LoadScene("ProximaCena"));
    }
}