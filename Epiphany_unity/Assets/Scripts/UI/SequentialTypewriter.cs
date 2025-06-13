using UnityEngine;
// using UnityEngine.UI; // Esta linha não é mais necessária para o TextMeshPro.
using TMPro; // <<< PASSO 1: Adicione esta linha para usar o TextMeshPro
using System.Collections;
using System.Collections.Generic;

public class SequentialTypewriter : MonoBehaviour
{
    //                                         vvvvvvvvvvvvvvv
    public TextMeshProUGUI targetText; // <<< PASSO 2: Mude o tipo de 'Text' para 'TextMeshProUGUI'
    //                                         ^^^^^^^^^^^^^^^
    public float typeSpeed = 0.05f;
    public float delayBetweenParagraphs = 1.5f;

    [TextArea(3, 10)]
    public List<string> paragraphs;

    // A referência ao seu TransitionManager continua a mesma.
    public CutsceneTransitionManager transitionManager;

    private string currentText = "";
    private int charIndex = 0;
    private int paragraphIndex = 0;
    private bool isTyping = false;

    // TODA A LÓGICA ABAIXO PERMANECE EXATAMENTE A MESMA.
    // A única diferença é que agora 'targetText' se refere a um componente TextMeshPro.

    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("O componente TextMeshProUGUI alvo não foi atribuído no GameObject: " + gameObject.name);
            enabled = false;
            return;
        }

        if (paragraphs == null || paragraphs.Count == 0)
        {
            Debug.LogWarning("A lista de parágrafos está vazia no GameObject: " + gameObject.name);
            enabled = false;
            return;
        }
        
        if (transitionManager == null)
        {
            Debug.LogError("O CutsceneTransitionManager não foi atribuído no SequentialTypewriter: " + gameObject.name);
        }

        StartCoroutine(PlaySequentialText());
    }

    IEnumerator PlaySequentialText()
    {
        while (paragraphIndex < paragraphs.Count)
        {
            isTyping = true;
            yield return StartCoroutine(TypeText(paragraphs[paragraphIndex]));
            yield return new WaitForSeconds(delayBetweenParagraphs);
            targetText.text = "";
            currentText = "";
            charIndex = 0;
            isTyping = false;
            paragraphIndex++;
        }

        Debug.Log("Todos os parágrafos foram exibidos.");
        
        if (transitionManager != null)
        {
            transitionManager.StartPostTextSequence();
        }
        else
        {
            Debug.LogWarning("Transição pós-texto não iniciada: transitionManager não configurado.");
        }
    }

    IEnumerator TypeText(string paragraph)
    {
        charIndex = 0;
        currentText = "";
        targetText.text = "";

        while (charIndex < paragraph.Length)
        {
            currentText += paragraph[charIndex];
            targetText.text = currentText;
            charIndex++;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    public void SetParagraphs(List<string> newParagraphs)
    {
        paragraphs = newParagraphs;
        paragraphIndex = 0;
        if (targetText != null) targetText.text = "";
        currentText = "";
        charIndex = 0;
        isTyping = false;
        StopAllCoroutines();
        if (gameObject.activeInHierarchy && paragraphs != null && paragraphs.Count > 0 && targetText != null)
        {
            StartCoroutine(PlaySequentialText());
        }
    }

    public void SetTypeSpeed(float newSpeed)
    {
        typeSpeed = newSpeed;
    }

    public void SetDelayBetweenParagraphs(float newDelay)
    {
        delayBetweenParagraphs = newDelay;
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public void SkipCurrentParagraph()
    {
        if (isTyping && paragraphIndex < paragraphs.Count)
        {
            StopCoroutine("TypeText");
            isTyping = false;
            targetText.text = paragraphs[paragraphIndex];
        }
    }

    public void SkipAll()
    {
        StopAllCoroutines();
        isTyping = false;
        
        if (paragraphs != null && paragraphs.Count > 0 && targetText != null)
        {
            paragraphIndex = paragraphs.Count;
            if (targetText != null) targetText.text = "";
            
            Debug.Log("Todos os parágrafos pulados. Iniciando transição.");
            if (transitionManager != null)
            {
                transitionManager.StartPostTextSequence();
            }
            else
            {
                Debug.LogWarning("Transição pós-texto não iniciada após SkipAll: transitionManager não configurado.");
            }
        }
    }
}