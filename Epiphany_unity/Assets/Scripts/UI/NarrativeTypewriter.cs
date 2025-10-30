using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class NarrativeTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 1.0f;

    public UnityEvent OnTypingCompleted;

    public void StartTyping(List<string> lines)
    {
        gameObject.SetActive(true);
        StartCoroutine(TypeLinesRoutine(lines));
    }

    private IEnumerator TypeLinesRoutine(List<string> lines)
    {
        targetText.text = "";
        foreach (string line in lines)
        {
            foreach (char letter in line.ToCharArray())
            {
                targetText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }
            yield return new WaitForSeconds(delayBetweenLines);
            targetText.text = ""; // Limpa para a próxima linha/fragmento
        }
        
        OnTypingCompleted.Invoke();
    }
}