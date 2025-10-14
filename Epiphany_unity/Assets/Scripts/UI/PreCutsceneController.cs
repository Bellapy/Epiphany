using UnityEngine;
using System.Collections;

public class PreCutsceneController : MonoBehaviour
{
    [Header("Referências da UI")]
    [Tooltip("Arraste o GameObject 'PainelInstrucoes' aqui.")]
    [SerializeField] private GameObject painelInstrucoes;

    [Header("Controle da Cutscene")]
    [Tooltip("Arraste o componente 'SequentialTypewriter' do objeto de texto da cutscene aqui.")]
    [SerializeField] private SequentialTypewriter sequentialTypewriter;

    [Header("Configurações de Tempo")]
    [Tooltip("Quanto tempo (em segundos) as instruções ficarão na tela.")]
    [SerializeField] private float tempoDeExibicao = 5.0f;

    void Start()
    {
        // Garante que o painel de instruções está visível no início.
        painelInstrucoes.SetActive(true);
        
        // MUITO IMPORTANTE: Desativamos o script que inicia a cutscene.
        // Assim, ele não vai começar a rodar o texto por conta própria.
        sequentialTypewriter.enabled = false;

        // Inicia a nossa sequência controlada.
        StartCoroutine(SequenciaDeInstrucoes());
    }

    private IEnumerator SequenciaDeInstrucoes()
    {
        // 1. Espera pelo tempo que definimos.
        yield return new WaitForSeconds(tempoDeExibicao);

        // 2. Esconde o painel de instruções.
        painelInstrucoes.SetActive(false);

        // 3. Agora sim, reativamos o script da cutscene para que ela comece.
        sequentialTypewriter.enabled = true;
    }
}
