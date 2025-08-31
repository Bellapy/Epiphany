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
    [SerializeField] private string mensagemErro = "Parece trancada... A combinação de luzes está incorreta.";
    [SerializeField] private string mensagemDica = "O lampião 3 parece importante...";

    private bool estaTrancada = true;
    private bool podeVerificar = true;
    private int tentativasErradas = 0;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !podeVerificar) return;

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
            tentativasErradas++;
            string mensagemAtual = (tentativasErradas >= 3) ? mensagemDica : mensagemErro;

            StartCoroutine(MostrarMensagemTemporaria(mensagemAtual));
            puzzleManager.ResetarTodosLampioes();
            StartCoroutine(CooldownVerificacao());
        }
    }

    private IEnumerator MostrarMensagemTemporaria(string mensagem)
    {
        ReflectionData dadosDaMensagem = new ReflectionData 
        { 
            reflectionLines = new List<string> { mensagem } 
        };
        
        DialogueManager.Instance.StartReflection(dadosDaMensagem);
        
        yield return new WaitForSeconds(3.0f);
        
        if (DialogueManager.Instance.IsDialogueBoxActive())
        {
            DialogueManager.Instance.CloseDialogueBox();
        }
    }

    private IEnumerator CooldownVerificacao()
    {
        podeVerificar = false;
        yield return new WaitForSeconds(2.0f);
        podeVerificar = true;
    }

    private void IniciarTransicao()
    {
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
        Debug.Log("A porta foi destrancada!");
    }
}