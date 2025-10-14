using UnityEngine;
using System.Collections; // Necessário para usar Corrotinas (IEnumerator)

[RequireComponent(typeof(NPCTourGuide))]
public class ActivateTourOnEnable : MonoBehaviour
{
    private NPCTourGuide tourGuide;

    void Awake() 
    { 
        tourGuide = GetComponent<NPCTourGuide>(); 
    }

    void OnEnable() 
    {
        // Em vez de chamar o método diretamente, iniciamos uma corrotina.
        StartCoroutine(StartTourAfterFrame());
    }

    private IEnumerator StartTourAfterFrame()
    {
        // <<< A CORREÇÃO ESTÁ AQUI >>>
        // Espera pelo final do frame atual.
        // Isso dá tempo para todos os outros objetos da cena (incluindo o Player)
        // completarem seus próprios ciclos de Awake() e Start().
        yield return new WaitForEndOfFrame();

        // Agora, no início do próximo frame, o tour é iniciado.
        // Neste ponto, o PlayerController já estará totalmente inicializado e encontrável.
        if (tourGuide != null)
        {
            tourGuide.StartTour();
        }
    }
}