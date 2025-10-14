using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortaPuzzle : MonoBehaviour
{
    [Header("Configuração do Puzzle")]
    [SerializeField] private PuzzleLuzesManager puzzleManager;

    [Header("Configuração de Transição")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string spawnPointInNextScene;
    
    [Header("Feedback para o Jogador")]
    [Tooltip("Lista de mensagens de feedback para tentativas erradas. A ordem importa.")]
    [SerializeField] private List<string> mensagensDeFeedback;

    private bool estaTrancada = true;
    private bool podeVerificar = true;
    private int tentativasErradas = 0;

    private void Awake()
    {
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // A verificação só acontece se o sistema estiver pronto.
        if (!other.CompareTag("Player") || !podeVerificar) return;
        
        VerificarPuzzle();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Adicionado para garantir a verificação caso o jogador pare dentro do trigger.
        if (!other.CompareTag("Player") || !podeVerificar) return;

        VerificarPuzzle();
    }

    private void VerificarPuzzle()
    {
        if (!estaTrancada)
        {
            IniciarTransicao();
            return;
        }
        
        if (puzzleManager.VerificarSolucao())
        {
            Destrancar();
            IniciarTransicao();
        }
        else
        {
            // Ao falhar, inicia a corrotina que gerencia todo o fluxo de feedback.
            StartCoroutine(ProcessarTentativaErrada());
        }
    }

    private IEnumerator ProcessarTentativaErrada()
    {
        // 1. Trava o sistema imediatamente. Nenhuma outra verificação pode acontecer.
        podeVerificar = false;

        // 2. Seleciona a mensagem correta.
        string mensagemAtual = "";
        if (mensagensDeFeedback != null && mensagensDeFeedback.Count > 0)
        {
            int indiceMensagem = Mathf.Min(tentativasErradas, mensagensDeFeedback.Count - 1);
            mensagemAtual = mensagensDeFeedback[indiceMensagem];
        }
        
        tentativasErradas++;

        // 3. Reseta os lampiões.
        puzzleManager.ResetarTodosLampioes();

        // 4. Mostra a mensagem, se houver uma.
        if (!string.IsNullOrEmpty(mensagemAtual))
        {
            ReflectionData dadosDaMensagem = ScriptableObject.CreateInstance<ReflectionData>();
            dadosDaMensagem.reflectionLines = new List<string> { mensagemAtual };
            DialogueManager.Instance.StartReflection(dadosDaMensagem);
            
            // 5. Espera um tempo fixo para a leitura.
            yield return new WaitForSeconds(3.0f);
            
            if (DialogueManager.Instance.IsDialogueBoxActive())
            {
                DialogueManager.Instance.CloseDialogueBox();
            }
            
            Destroy(dadosDaMensagem);
        }
        
        // 6. Destrava o sistema, permitindo uma nova tentativa.
        podeVerificar = true;
    }

    private void IniciarTransicao()
    {
        // Garante que a transição só seja chamada uma vez.
        if (!podeVerificar) return;
        podeVerificar = false;

        if (FadeController.Instance != null) {
            FadeController.Instance.StartFadeOut(() => {
                GameManager.Instance.SetNextSpawnPoint(spawnPointInNextScene);
                GameManager.Instance.LoadScene(nextSceneName);
            });
        } else {
            GameManager.Instance.SetNextSpawnPoint(spawnPointInNextScene);
            GameManager.Instance.LoadScene(nextSceneName);
        }
    }

    public void Destrancar()
    {
        estaTrancada = false;
    }
}