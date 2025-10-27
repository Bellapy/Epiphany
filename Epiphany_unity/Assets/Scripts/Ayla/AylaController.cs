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
    [SerializeField] private float zoomInSize = 1.2f;
    [SerializeField] private float zoomOutSize;
    [SerializeField] private float zoomSpeed = 1.0f;

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
    
    public void IniciarCutsceneEncontro()
    {
        if (currentState != SceneState.Idle) return;
        
        StartCoroutine(SequenciaEncontro());
    }

    private void HandleDialogueEnd()
    {
        if (DialogueManager.Instance == null) return;

        if (currentState == SceneState.ShowingIntro)
        {
            currentState = SceneState.AwaitingFollowPrompt;
            DialogueManager.Instance.StartDialogue(dialogoMeSiga);
        }
        else if (currentState == SceneState.AwaitingFollowPrompt)
        {
            currentState = SceneState.Done;
            StartCoroutine(SequenciaCaminhada());
        }
    }

    private IEnumerator SequenciaEncontro()
    {
        if (DialogueManager.Instance == null) yield break;

        currentState = SceneState.ShowingIntro;
        StartCoroutine(DoZoom(zoomInSize));
        aylaLastVerticalDirection = -1;
        AtualizarAnimacaoAyla(Vector2.zero);
        yield return new WaitForSeconds(0.75f);
        DialogueManager.Instance.StartDialogue(dialogoEncontro);
    }

    private IEnumerator SequenciaCaminhada()
    {
        StartCoroutine(DoZoom(zoomOutSize));
        yield return new WaitForSeconds(zoomSpeed);

        if(playerController != null) playerController.EnableMovement();
        
        Vector2 direcao = (pontoDestinoEscada.position - aylaTransform.position).normalized;
        AtualizarAnimacaoAyla(direcao);
        
        yield return new WaitForSeconds(0.25f);

        while (Vector3.Distance(aylaTransform.position, pontoDestinoEscada.position) > 0.1f)
        {
            aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestinoEscada.position, velocidadeCaminhada * Time.deltaTime);
            yield return null;
        }

        aylaTransform.position = pontoDestinoEscada.position;
        AtualizarAnimacaoAyla(Vector2.zero);
        
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
        if (virtualCamera == null) yield break;

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
        if (aylaAnimator == null) return; 
        
        int estadoMovimento; 
        bool estaMovendo = direcaoMovimento.magnitude > 0.1f;
        
        if (estaMovendo) 
        {
            float moveX = direcaoMovimento.x; 
            float moveY = direcaoMovimento.y;
            
            if (Mathf.Abs(moveY) > Mathf.Abs(moveX)) 
            {
                if (moveY > 0) 
                { 
                    estadoMovimento = 3; 
                    aylaLastVerticalDirection = 1; 
                } 
                else 
                { 
                    estadoMovimento = 1; 
                    aylaLastVerticalDirection = -1; 
                }
            } 
            else 
            {
                estadoMovimento = 5; 
                if (aylaSpriteRenderer != null)
                {
                    aylaSpriteRenderer.flipX = moveX < 0;
                }
            }
        } 
        else 
        {
            if (aylaLastVerticalDirection == 1) 
            { 
                estadoMovimento = 2; 
            } 
            else 
            { 
                estadoMovimento = 0; 
            }
        }
        aylaAnimator.SetInteger("MovementState", estadoMovimento);
    }
}