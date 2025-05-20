using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SequentialTypewriter : MonoBehaviour
{
    public Text targetText; // Arraste o componente Text da UI para cá no Inspector
    public float typeSpeed = 0.05f; // Tempo entre o aparecimento de cada letra
    public float delayBetweenParagraphs = 1.5f; // Tempo de espera entre o desaparecimento de um parágrafo e o surgimento do próximo

    [TextArea(3, 10)] // Permite escrever parágrafos maiores no Inspector
    public List<string> paragraphs; // A lista de parágrafos a serem exibidos sequencialmente

    private string currentText = "";
    private int charIndex = 0;
    private int paragraphIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("O componente Text alvo não foi atribuído no GameObject: " + gameObject.name);
            return;
        }

        if (paragraphs == null || paragraphs.Count == 0)
        {
            Debug.LogWarning("A lista de parágrafos está vazia no GameObject: " + gameObject.name);
            return;
        }

        StartCoroutine(PlaySequentialText());
    }

    IEnumerator PlaySequentialText()
    {
        while (paragraphIndex < paragraphs.Count)
        {
            isTyping = true;
            yield return StartCoroutine(TypeText(paragraphs[paragraphIndex]));

            // Espera um pouco antes de apagar o texto e começar o próximo parágrafo
            yield return new WaitForSeconds(delayBetweenParagraphs);

            // Apaga o texto
            targetText.text = "";
            currentText = "";
            charIndex = 0;
            isTyping = false;

            paragraphIndex++;
        }

        // Opcional: Faça algo quando todos os parágrafos forem exibidos
        Debug.Log("Todos os parágrafos foram exibidos.");
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
        targetText.text = "";
        currentText = "";
        charIndex = 0;
        isTyping = false;
        StopAllCoroutines();
        StartCoroutine(PlaySequentialText());
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
        if (isTyping)
        {
            StopCoroutine(TypeText(paragraphs[paragraphIndex]));
            isTyping = false;
            targetText.text = paragraphs[paragraphIndex];
        }
    }

    public void SkipAll()
    {
        StopAllCoroutines();
        isTyping = false;
        paragraphIndex = paragraphs.Count;
        string allText = "";
        foreach (string paragraph in paragraphs)
        {
            allText += paragraph + "\n\n";
        }
        targetText.text = allText.TrimEnd('\n');
    }
}