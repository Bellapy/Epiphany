using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class EndingSceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private SequentialTypewriter endTextTypewriter;
    [SerializeField] private GameObject exitButton;

    // --- Ciclo de Vida da Unity ---
    
    void Start()
    {
        // Garante que o texto e o botão comecem invisíveis
        if(endTextTypewriter != null) endTextTypewriter.gameObject.SetActive(false);
        if(exitButton != null) exitButton.SetActive(false);
        
        // Inicia a sequência principal
        StartCoroutine(FullEndingSequence());
    }

    private void OnEnable()
    {
        // Começa a "ouvir" o evento de quando o vídeo termina
        if(videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void OnDisable()
    {
        // Para de "ouvir" para evitar erros
        if(videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    // --- Lógica da Sequência ---

    private IEnumerator FullEndingSequence()
    {
        // 1. Prepara e espera o vídeo ficar pronto
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        
        // 2. Inicia o Fade In da cena
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeIn();
        }
        
        // 3. Toca o vídeo
        videoPlayer.Play();
    }
    
    /// <summary>
    /// Este método é chamado AUTOMATICAMENTE pelo VideoPlayer quando ele termina de tocar.
    /// </summary>
    private void OnVideoEnd(VideoPlayer source)
    {
        Debug.Log("O vídeo terminou. Iniciando sequência de texto final.");
        StartCoroutine(FinalTextSequence());
    }

    private IEnumerator FinalTextSequence()
    {
        // 1. Inicia o Fade Out para escurecer a tela
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeOut(null);
        }

        // Espera o fade out terminar
        // A duração do fade está no FadeController, vamos assumir 1 segundo.
        yield return new WaitForSeconds(1.0f);

        // 2. Com a tela preta, ativa o texto e o botão
        if(endTextTypewriter != null) endTextTypewriter.gameObject.SetActive(true);
        if(exitButton != null) exitButton.SetActive(true);
    }
}