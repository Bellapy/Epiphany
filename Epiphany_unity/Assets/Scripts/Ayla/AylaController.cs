using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class AylaCutsceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private Transform aylaTransform;
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private Transform playerTransform; 

    [Header("Diálogos da Cena")]
    [SerializeField] private DialogueData dialogoEncontro;
    [SerializeField] private DialogueData dialogoMeSiga; 

    [Header("Pontos de Roteiro")]
    [SerializeField] private Transform pontoDestinoEscada;
    [SerializeField] private float velocidadeCaminhada = 1.0f;

    [Header("Configurações de Zoom")]
    // <<< VALOR PADRÃO AJUSTADO >>>
    [SerializeField] private float zoomInSize = 1.2f; // Valor menor para aproximar
    [SerializeField] private float zoomOutSize;
    [SerializeField] private float zoomSpeed = 1.0f;

    // A nossa máquina de estados para controlar o fluxo
    private enum SceneState { Idle, ShowingIntro, AwaitingFollowPrompt, Done }
    private SceneState currentState = SceneState.Idle;

    private int aylaLastVerticalDirection = 1;

    private void Awake()
    {
        aylaLastVerticalDirection = 1;
        AtualizarAnimacaoAyla(Vector2.zero);

        if (virtualCamera != null)
        {
            zoomOutSize = virtualCamera.Lens.OrthographicSize;
        }
    }

    private void OnEnable() { DialogueManager.OnDialogueEnd += HandleDialogueEnd; }
    private void OnDisable() { DialogueManager.OnDialogueEnd -= HandleDialogueEnd; }
    
    public void IniciarCutsceneEncontro()
    {
        if (currentState != SceneState.Idle) return;
        
        StartCoroutine(SequenciaEncontro());
    }

    // <<< HANDLEDIALOGUEEND CORRIGIDO E COMPLETO >>>
    private void HandleDialogueEnd()
    {
        // Se o diálogo de INTRODUÇÃO acabou...
        if (currentState == SceneState.ShowingIntro)
        {
            // ...agora vamos para o diálogo "Me Siga".
            currentState = SceneState.AwaitingFollowPrompt;
            DialogueManager.Instance.StartDialogue(dialogoMeSiga);
        }
        // Se o diálogo "Me Siga" acabou...
        else if (currentState == SceneState.AwaitingFollowPrompt)
        {
            // ...agora é hora de andar.
            currentState = SceneState.Done;
            StartCoroutine(SequenciaCaminhada());
        }
    }

    private IEnumerator SequenciaEncontro()
    {
        currentState = SceneState.ShowingIntro;
        StartCoroutine(DoZoom(zoomInSize));
        aylaLastVerticalDirection = -1;
        AtualizarAnimacaoAyla(Vector2.zero);
        yield return new WaitForSeconds(0.75f);
        DialogueManager.Instance.StartDialogue(dialogoEncontro);
    }

    private IEnumerator SequenciaCaminhada()
{
    // 1. O zoom volta ao normal.
    StartCoroutine(DoZoom(zoomOutSize));
    yield return new WaitForSeconds(zoomSpeed);

    // 2. O jogador é liberado.
    if(playerController != null) playerController.EnableMovement();
    
    // <<< A LÓGICA CORRIGIDA COMEÇA AQUI >>>

    // 3. PRIMEIRO, calculamos a direção para onde ela DEVE ir.
    Vector2 direcao = (pontoDestinoEscada.position - aylaTransform.position).normalized;

    // 4. AGORA, com base na direção, definimos a animação de caminhada.
    // Isso já vai fazê-la virar para a esquerda e começar a animar as pernas.
    AtualizarAnimacaoAyla(direcao);
    
    // Pequena pausa dramática antes de ela realmente sair do lugar. (Opcional, mas bom para o ritmo)
    yield return new WaitForSeconds(0.25f);

    // 5. FINALMENTE, movemos a Ayla, que JÁ está animada e virada para o lado certo.
    while (Vector3.Distance(aylaTransform.position, pontoDestinoEscada.position) > 0.1f)
    {
        aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestinoEscada.position, velocidadeCaminhada * Time.deltaTime);
        yield return null;
    }

    // 6. Chegou! Parar a Ayla e garantir a pose final.
    aylaTransform.position = pontoDestinoEscada.position;
    AtualizarAnimacaoAyla(Vector2.zero); // Manda parar
    
    // ... O resto da lógica de subir a escada continua aqui ...
    aylaAnimator.SetInteger("MovementState", 3);
    
    float tempoDeSubida = 2.0f;
    float tempoPassado = 0f;
    Vector3 posInicial = aylaTransform.position;
    Vector3 posFinalSubida = posInicial + new Vector3(0, 3, 0);

    while(tempoPassado < tempoDeSubida)
    {
        aylaTransform.position = Vector3.Lerp(posInicial, posFinalSubida, tempoPassado / tempoDeSubida);
        tempoPassado += Time.deltaTime;
        yield return null;
    }
    
    aylaTransform.gameObject.SetActive(false);
}
    private IEnumerator DoZoom(float targetSize)
    {
        float startSize = virtualCamera.Lens.OrthographicSize;
        float timer = 0f;

        while(timer < zoomSpeed)
        {
            timer += Time.deltaTime;
            float newSize = Mathf.Lerp(startSize, targetSize, timer / zoomSpeed);
            virtualCamera.Lens.OrthographicSize = newSize;
            yield return null;
        }

        virtualCamera.Lens.OrthographicSize = targetSize;
    }

    private void AtualizarAnimacaoAyla(Vector2 direcaoMovimento)
    {
        if (aylaAnimator == null) return; int estadoMovimento; bool estaMovendo = direcaoMovimento.magnitude > 0.1f;
        if (estaMovendo) {
            float moveX = direcaoMovimento.x; float moveY = direcaoMovimento.y;
            if (Mathf.Abs(moveY) > Mathf.Abs(moveX)) {
                if (moveY > 0) { estadoMovimento = 3; aylaLastVerticalDirection = 1; } else { estadoMovimento = 1; aylaLastVerticalDirection = -1; }
            } else {
                estadoMovimento = 5; aylaSpriteRenderer.flipX = moveX < 0;
            }
        } else {
            if (aylaLastVerticalDirection == 1) { estadoMovimento = 2; } else { estadoMovimento = 0; }
        }
        aylaAnimator.SetInteger("MovementState", estadoMovimento);
    }
}