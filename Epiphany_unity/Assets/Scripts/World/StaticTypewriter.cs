using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

// Evento para notificar se está digitando ou não (usado pelo StoryStationController).
[System.Serializable]
public class TypingStateChangedEvent : UnityEvent<bool> {}

public class StaticTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private float typeSpeed = 0.05f;

    // Evento para notificar quando a digitação termina (usado pelo EndingSceneController).
    public UnityEvent OnTypingCompleted;
    
    // Evento para notificar o estado da digitação (usado pelo StoryStationController).
    public TypingStateChangedEvent OnTypingStateChanged;

    private Coroutine typingCoroutine;
    private string fullText; // Variável para armazenar o texto completo para a função SkipToEnd.

    public void StartTyping(string text)
    {
        fullText = text;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextRoutine(fullText));
    }

    // --- FUNÇÃO ADICIONADA DE VOLTA ---
    public void SkipToEnd()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            targetText.text = fullText;
            OnTypingStateChanged.Invoke(false); // Notifica que terminou de digitar.
            OnTypingCompleted.Invoke(); // Também notifica que a digitação foi concluída.
        }
    }
    // --- FIM DA ADIÇÃO ---

    private IEnumerator TypeTextRoutine(string text)
    {
        OnTypingStateChanged.Invoke(true); // Notifica que começou a digitar.
        targetText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            targetText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        OnTypingStateChanged.Invoke(false); // Notifica que terminou de digitar.
        OnTypingCompleted.Invoke(); // Notifica que a digitação foi concluída.
    }
}