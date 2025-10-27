using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PortaPuzzle : MonoBehaviour
{
    [Header("Configuração do Puzzle")]
    [Tooltip("Arraste os 4 lampiões aqui, NA ORDEM CORRETA (1, 2, 3, 4).")]
    [SerializeField] private List<LampiaoController> lampioesDoPuzzle;

    [Header("Configuração de Transição")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string spawnPointInNextScene;
    
    [Header("Feedback para o Jogador")]
    [Tooltip("Lista de mensagens de feedback para tentativas erradas.")]
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !podeVerificar) return;
        VerificarSolucao();
    }

    private void VerificarSolucao()
    {
        if (!estaTrancada)
        {
            IniciarTransicao();
            return;
        }
        
        bool solucaoCorreta = lampioesDoPuzzle.Count >= 4 &&
                              lampioesDoPuzzle[0].EstaAceso() &&
                              lampioesDoPuzzle[1].EstaAceso() &&
                              lampioesDoPuzzle[2].EstaAceso() &&
                              !lampioesDoPuzzle[3].EstaAceso();

        if (solucaoCorreta)
        {
            Destrancar();
            IniciarTransicao();
        }
        else
        {
            StartCoroutine(ProcessarTentativaErrada());
        }
    }

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