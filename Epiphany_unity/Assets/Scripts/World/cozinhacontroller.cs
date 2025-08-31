using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine; // Adicionado para o controle da câmera

public class CozinhaSceneController : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choiceButtonsContainer;
    [SerializeField] private List<Button> choiceButtons;
    [SerializeField] private Sprite playerPortrait;

    [Header("Referências de Atores")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private Transform aylaTransform;
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;
    [SerializeField] private Transform pontoDestinoPorta;
    [SerializeField] private float velocidadeCaminhada = 1.0f;

    [Header("Câmera e Zoom")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float zoomInSize = 1.2f;
    [SerializeField] private float zoomSpeed = 1.0f;
    private float zoomOutSize;

    [Header("Conteúdo da Conversa")]
    [SerializeField] private DialogueData dialogoIntroducaoAyla;
    [SerializeField] private List<string> todasAsPerguntas;
    [SerializeField] private List<DialogueData> dialogosDeResposta;
    [SerializeField] private DialogueData dialogoFinalAyla;

    private List<string> perguntasDisponiveis;
    private bool finalDialogueStarted = false;

    private void OnEnable() { DialogueManager.OnDialogueEnd += HandleDialogueEnd; }
    private void OnDisable() { DialogueManager.OnDialogueEnd -= HandleDialogueEnd; }

    void Start()
    {
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", true);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (virtualCamera != null) zoomOutSize = virtualCamera.Lens.OrthographicSize;
    }
    
    public void IniciarConversa()
    {
        perguntasDisponiveis = new List<string>(todasAsPerguntas);
        finalDialogueStarted = false;
        
        StartCoroutine(DoZoom(zoomInSize)); // << PONTO 2: DÁ ZOOM
        
        dialoguePanel.SetActive(true);
        choiceButtonsContainer.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        DialogueManager.Instance.StartDialogue(dialogoIntroducaoAyla);
    }
    
    private void HandleDialogueEnd()
    {
        if (finalDialogueStarted)
        {
            StartCoroutine(SequenciaFinalAyla()); // << PONTO 3: AYLA ANDA
            return;
        }

        if (perguntasDisponiveis.Count > 0)
        {
            ApresentarEscolhas();
        }
        else
        {
            finalDialogueStarted = true;
            DialogueManager.Instance.StartDialogue(dialogoFinalAyla);
        }
    }
    
    private void ApresentarEscolhas()
    {
        dialoguePanel.SetActive(true);
        dialogueText.gameObject.SetActive(false);
        choiceButtonsContainer.SetActive(true);
        speakerNameText.text = "Você";
        speakerPortrait.sprite = playerPortrait;

        Button botao1 = choiceButtons[0];
        botao1.gameObject.SetActive(true);
        botao1.GetComponentInChildren<TextMeshProUGUI>().text = perguntasDisponiveis[0];
        botao1.onClick.RemoveAllListeners();
        string pergunta1 = perguntasDisponiveis[0];
        botao1.onClick.AddListener(() => EscolherPergunta(pergunta1));

        Button botao2 = choiceButtons[1];
        if (perguntasDisponiveis.Count > 1) {
            botao2.gameObject.SetActive(true);
            botao2.GetComponentInChildren<TextMeshProUGUI>().text = perguntasDisponiveis[1];
            botao2.onClick.RemoveAllListeners();
            string pergunta2 = perguntasDisponiveis[1];
            botao2.onClick.AddListener(() => EscolherPergunta(pergunta2));
        } else {
            botao2.gameObject.SetActive(false);
        }

        // << PONTO 1: NAVEGAÇÃO POR SETAS >>
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); // Limpa a seleção anterior
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
    }

    private void EscolherPergunta(string perguntaEscolhida)
    {
        choiceButtonsContainer.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        int indiceOriginal = todasAsPerguntas.IndexOf(perguntaEscolhida);
        perguntasDisponiveis.Remove(perguntaEscolhida);
        DialogueManager.Instance.StartDialogue(dialogosDeResposta[indiceOriginal]);
    }

    private IEnumerator SequenciaFinalAyla()
    {
        // Afasta o zoom de volta ao normal
        StartCoroutine(DoZoom(zoomOutSize));
        yield return new WaitForSeconds(zoomSpeed);
        
        // Ayla se levanta
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", false);
        yield return new WaitForSeconds(1.5f);

        // Ayla anda até a porta
        Vector2 direcao = (pontoDestinoPorta.position - aylaTransform.position).normalized;
        aylaAnimator.SetInteger("MovementState", 5);
        aylaSpriteRenderer.flipX = direcao.x < 0;
        while (Vector3.Distance(aylaTransform.position, pontoDestinoPorta.position) > 0.1f)
        {
            aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestinoPorta.position, velocidadeCaminhada * Time.deltaTime);
            yield return null;
        }
        aylaTransform.position = pontoDestinoPorta.position;
        aylaAnimator.SetInteger("MovementState", 4);
        
        // Aqui, a Ayla espera pela Player. Podemos adicionar mais lógica no futuro.
        Debug.Log("Ayla chegou na porta e está esperando.");
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
}