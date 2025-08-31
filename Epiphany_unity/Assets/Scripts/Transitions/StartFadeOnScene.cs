using UnityEngine;
using System.Collections;

public class StartFadeOnScene : MonoBehaviour
{
    // <<< NOSSO NOVO INTERRUPTOR >>>
    [Tooltip("Marque esta caixa para que a cena comece com um efeito de fade in.")]
    [SerializeField] private bool fadeInOnStart = true;

    IEnumerator Start()
    {
        // Espera um frame para garantir que o FadeController já foi inicializado.
        yield return null; 

        // <<< A NOVA CONDIÇÃO >>>
        // Só executa o fade se o nosso interruptor estiver ligado.
        if (fadeInOnStart)
        {
            if (FadeController.Instance != null)
            {
                FadeController.Instance.StartFadeIn();
            }
            else
            {
                Debug.LogWarning("FadeController não encontrado. Não foi possível fazer o fade in.");
            }
        }
    }
}