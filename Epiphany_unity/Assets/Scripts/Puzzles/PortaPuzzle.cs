// Em Scripts/World/PortaPuzzle.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PortaPuzzle : MonoBehaviour
{
    [Header("Configuração do Puzzle")]
    [SerializeField] private List<LampiaoController> lampioesDoPuzzle;
    
    // <<< CAMPO DA BARREIRA REMOVIDO, NÃO É MAIS NECESSÁRIO AQUI >>>

    [Header("Configuração de Transição")]
    // <<< VALORES ALTERADOS PARA REFLETIR O NOVO FLUXO >>>
    [Tooltip("A cena para onde o jogador é enviado APÓS resolver o puzzle.")]
    [SerializeField] private string nextSceneName = "cozinha"; 
    [Tooltip("O nome do ponto de spawn na próxima cena (cozinha).")]
    [SerializeField] private string spawnPointInNextScene = "SpawnFromCorredor"; 
    
    [Header("Feedback para o Jogador")]
    [SerializeField] private List<string> mensagensDeFeedback;

    private bool estaTrancada = true;
    private bool podeVerificar = true;
    private int tentativasErradas = 0;
    private FadeController fadeController;

    private void Awake()
    {
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();
        
        // <<< NOVA LÓGICA NO START >>>
        // Se o puzzle já foi resolvido, este componente não precisa fazer mais nada.
        if (GameManager.Instance != null && GameManager.Instance.HasSolvedCorridorPuzzle)
        {
            estaTrancada = false;
            // Opcional: Desativar o componente para otimização.
            // this.enabled = false; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !podeVerificar) return;
        VerificarSolucao();
    }

    private void VerificarSolucao()
    {
        // Se a porta já está destrancada, não faz nada. A transição para a floresta é em outro objeto.
        if (!estaTrancada)
        {
            return;
        }
        
        bool solucaoCorreta = lampioesDoPuzzle.Count >= 4 &&
                              lampioesDoPuzzle[0].EstaAceso() &&
                              lampioesDoPuzzle[1].EstaAceso() &&
                              lampioesDoPuzzle[2].EstaAceso() &&
                              !lampioesDoPuzzle[3].EstaAceso();

        if (solucaoCorreta)
        {
            // <<< LÓGICA DE SUCESSO MODIFICADA >>>
            
            // 1. Define a flag global
            if (GameManager.Instance != null)
            {
                GameManager.Instance.HasSolvedCorridorPuzzle = true;
            }
            
            // 2. Destranca localmente
            Destrancar();
            
            // 3. Inicia a transição para a COZINHA
            IniciarTransicao();
        }
        else
        {
            StartCoroutine(ProcessarTentativaErrada());
        }
    }

    // ... (A corrotina ProcessarTentativaErrada permanece a mesma) ...
    private IEnumerator ProcessarTentativaErrada()
    {
        podeVerificar = false;
        string mensagemAtual = "";
        if (mensagensDeFeedback != null && mensagensDeFeedback.Count > 0)
        {
            int indiceMensagem = Mathf.Min(tentativasErradas, mensagensDeFeedback.Count - 1);
            mensagemAtual = mensagensDeFeedback[indiceMensagem];
        }
        tentativasErradas++;
        foreach (var lampiao in lampioesDoPuzzle)
        {
            lampiao.ResetarLampiao();
        }
        if (!string.IsNullOrEmpty(mensagemAtual) && DialogueManager.Instance != null)
        {
            ReflectionData dadosDaMensagem = ScriptableObject.CreateInstance<ReflectionData>();
            dadosDaMensagem.reflectionLines = new List<string> { mensagemAtual };
            DialogueManager.Instance.StartReflectionWithFadeOut(dadosDaMensagem, 2.0f, 0.5f);
            Destroy(dadosDaMensagem, 3.0f);
        }
        yield return new WaitForSeconds(1.0f);
        podeVerificar = true;
    }

    private void IniciarTransicao()
    {
        if (!podeVerificar || GameManager.Instance == null) return;
        podeVerificar = false;

        if (fadeController != null)
        {
            fadeController.StartFadeOut(() => {
                GameManager.Instance.SetNextSpawnPoint(spawnPointInNextScene);
                GameManager.Instance.LoadScene(nextSceneName);
            });
        }
        else
        {
            GameManager.Instance.SetNextSpawnPoint(spawnPointInNextScene);
            GameManager.Instance.LoadScene(nextSceneName);
        }
    }

    public void Destrancar()
    {
        estaTrancada = false;
    }
}