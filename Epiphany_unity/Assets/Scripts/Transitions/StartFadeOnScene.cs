using UnityEngine;
using System.Collections;

public class StartFadeOnScene : MonoBehaviour
{
    IEnumerator Start()
    {
        // Espera um frame para garantir que o FadeController.Instance foi inicializado
        yield return null;

        // Agora faz o fade in suavemente
        FadeController.Instance.StartFadeIn();
    }
}


