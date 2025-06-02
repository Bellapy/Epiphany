using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SequentialTypewriter : MonoBehaviour
{
    public Text targetText;
    public float typeSpeed = 0.05f;
    public float delayBetweenParagraphs = 1.5f;

    [TextArea(3, 10)]
    public List<string> paragraphs;

    // ---- NOVA LINHA ----
    public CutsceneTransitionManager transitionManager; // Arraste o GameObject com o CutsceneTransitionManager aqui

    private string currentText = "";
    private int charIndex = 0;
    private int paragraphIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("O componente Text alvo não foi atribuído no GameObject: " + gameObject.name);
            enabled = false; // Desabilita o script se não houver texto
            return;
        }

        if (paragraphs == null || paragraphs.Count == 0)
        {
            Debug.LogWarning("A lista de parágrafos está vazia no GameObject: " + gameObject.name);
            enabled = false; // Desabilita o script se não houver parágrafos
            return;
        }

        // ---- NOVA LINHA ----
        if (transitionManager == null)
        {
            Debug.LogError("O CutsceneTransitionManager não foi atribuído no SequentialTypewriter: " + gameObject.name);
            // Você pode optar por desabilitar o script aqui também, ou apenas avisar.
            // enabled = false; 
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

        // ---- BLOCO MODIFICADO/ADICIONADO ----
        if (transitionManager != null)
        {
            transitionManager.StartPostTextSequence();
        }
        else
        {
            Debug.LogWarning("Transição pós-texto não iniciada: transitionManager não configurado.");
        }
        // ---- FIM DO BLOCO ----
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

    // ... (resto do seu script: SetParagraphs, SetTypeSpeed, etc. permanecem iguais) ...

    public void SetParagraphs(List<string> newParagraphs)
    {
        paragraphs = newParagraphs;
        paragraphIndex = 0;
        if (targetText != null) targetText.text = "";
        currentText = "";
        charIndex = 0;
        isTyping = false;
        StopAllCoroutines(); // Para garantir que não haja corrotinas antigas rodando
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
        if (isTyping && paragraphIndex < paragraphs.Count) // Adicionada verificação de paragraphIndex
        {
            StopCoroutine("TypeText"); // Pare a corrotina específica pelo nome
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
            // Mostra o último parágrafo ou todos, dependendo da preferência
            // Aqui, vamos apenas mostrar o último parágrafo completo
            // Se quiser mostrar todos, teria que concatená-los.
            // Para o propósito de pular para a transição, apenas terminar o texto atual é suficiente
            // ou avançar paragraphIndex para o final e chamar a transição.

            // Opção 1: Simplesmente preencher o texto com o último parágrafo
            // targetText.text = paragraphs[paragraphs.Count - 1];

            // Opção 2: Avançar para o final e chamar a transição (mais direto)
            paragraphIndex = paragraphs.Count; // Marca como se todos tivessem sido exibidos
            if (targetText != null) targetText.text = ""; // Limpa o texto atual
            
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