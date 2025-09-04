using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Referências de Atores e Roteiro")]
    [SerializeField] private Animator aylaAnimator;
    [SerializeField] private Transform aylaTransform;
    [SerializeField] private SpriteRenderer aylaSpriteRenderer;
    [SerializeField] private Transform pontoDestinoPorta;
    [SerializeField] private float velocidadeCaminhada = 1.0f;
    [SerializeField] private Transform aylaPontoDePartida;

    [Header("Conteúdo da Conversa")]
    [SerializeField] private DialogueData dialogoIntroducaoAyla;
    [SerializeField] private List<string> todasAsPerguntas;
    [SerializeField] private List<DialogueData> dialogosDeResposta;
    [SerializeField] private DialogueData dialogoFinalAyla;

    private List<string> perguntasDisponiveis;
    private bool finalDialogueStarted = false;
    private bool isConversationActive = false; // A trava de segurança

    private void OnEnable() { DialogueManager.OnDialogueEnd += HandleDialogueEnd; }
    private void OnDisable() { DialogueManager.OnDialogueEnd -= HandleDialogueEnd; }

    void Start()
    {
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", true);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
    
    public void IniciarConversa()
    {
        isConversationActive = true; // Liga o "interruptor" da conversa
        
        perguntasDisponiveis = new List<string>(todasAsPerguntas);
        finalDialogueStarted = false;
        
        dialoguePanel.SetActive(true);
        choiceButtonsContainer.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        DialogueManager.Instance.StartDialogue(dialogoIntroducaoAyla);
    }
    
    private void HandleDialogueEnd()
    {
        if (!isConversationActive) return; // Ignora eventos que não são da conversa principal

        if (finalDialogueStarted)
        {
            isConversationActive = false; // Desliga o "interruptor"
            StartCoroutine(SequenciaFinalAyla());
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
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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

    // <<< SEQUÊNCIA FINAL COM A LÓGICA DE FADE E TRANSIÇÃO RESTAURADA >>>
    private IEnumerator SequenciaFinalAyla()
    {
        // Ayla se levanta
        if (aylaAnimator != null) aylaAnimator.SetBool("isSitting", false);
        
        // Inicia o Fade Out IMEDIATAMENTE.
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeOut(null);
        }

        // Espera um pouco para a animação de levantar acontecer enquanto a tela escurece
        float tempoParaLevantar = 1.5f;
        float timer = 0f;
        Vector3 posInicialSentada = aylaTransform.position;
        while (timer < tempoParaLevantar)
        {
            aylaTransform.position = Vector3.Lerp(posInicialSentada, aylaPontoDePartida.position, timer / tempoParaLevantar);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ayla anda até a porta (isso acontecerá enquanto a tela já está escura ou escurecendo)
        Vector2 direcao = (pontoDestinoPorta.position - aylaTransform.position).normalized;
        aylaAnimator.SetInteger("MovementState", 5);
        aylaSpriteRenderer.flipX = direcao.x < 0;
        while (Vector3.Distance(aylaTransform.position, pontoDestinoPorta.position) > 0.1f)
        {
            aylaTransform.position = Vector3.MoveTowards(aylaTransform.position, pontoDestinoPorta.position, velocidadeCaminhada * Time.deltaTime);
            yield return null;
        }

        // Com a tela já preta, agora damos a ordem para trocar de cena.
        Debug.Log("Transição final com fade. Carregando a EndingScene.");
        GameManager.Instance.LoadScene("EndingScene");
    }
}