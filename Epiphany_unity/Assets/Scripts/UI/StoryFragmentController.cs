using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class StoryFragmentController : MonoBehaviour
{
    [Header("Referências Visuais")]
    [SerializeField] private CanvasGroup plaqueCanvasGroup; // Placa e texto
    [SerializeField] private Animator glowAnimator; // Círculo brilhante

    [Header("Conteúdo Narrativo")]
    [SerializeField] private NarrativeTypewriter narrativeTypewriter;
    [TextArea(3, 10)]
    [SerializeField] private List<string> storyLines;

    public UnityEvent OnFragmentCompleted;

    private bool hasBeenTriggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (plaqueCanvasGroup != null) plaqueCanvasGroup.alpha = 0;
        if (glowAnimator != null) glowAnimator.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true;
        StartCoroutine(FragmentSequence());
    }

    private IEnumerator FragmentSequence()
    {
        // Ativa o brilho pulsante
        if (glowAnimator != null)
        {
            glowAnimator.gameObject.SetActive(true);
            // Supondo que a animação de pulsar comece automaticamente
        }

        // Fade in da placa
        yield return StartCoroutine(FadeCanvasGroup(plaqueCanvasGroup, 1f, 0.5f));

        // Inicia a escrita do texto
        if (narrativeTypewriter != null)
        {
            narrativeTypewriter.OnTypingCompleted.AddListener(HandleTypingCompleted);
            narrativeTypewriter.StartTyping(storyLines);
        }
    }

    private void HandleTypingCompleted()
    {
        narrativeTypewriter.OnTypingCompleted.RemoveListener(HandleTypingCompleted);
        StartCoroutine(EndFragmentSequence());
    }

    private IEnumerator EndFragmentSequence()
    {
        yield return new WaitForSeconds(1.0f);

        // Fade out da placa
        yield return StartCoroutine(FadeCanvasGroup(plaqueCanvasGroup, 0f, 0.5f));
        
        // Notifica o controlador da cena que este fragmento terminou
        OnFragmentCompleted.Invoke();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        cg.alpha = targetAlpha;
    }
}