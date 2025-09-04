using UnityEngine;
using TMPro; // Necessário para TextMeshPro
using System.Collections;
using System.Collections.Generic;

public class StaticTypewriter : MonoBehaviour
{
    [Header("Configuração do Texto")]
    [Tooltip("Arraste o componente TextMeshProUGUI que está neste mesmo GameObject aqui.")]
    [SerializeField] private TextMeshProUGUI targetText;
    
    [Tooltip("A velocidade com que o texto é digitado (segundos por letra).")]
    [SerializeField] private float typeSpeed = 0.08f;

    [Header("Conteúdo")]
    [Tooltip("A lista de parágrafos a serem escritos. Geralmente, apenas um.")]
    [SerializeField] private List<string> paragraphs;

    // O método Start é chamado quando o objeto é ativado.
    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("Target Text não foi atribuído no StaticTypewriter!", this.gameObject);
            return;
        }

        // Inicia a corrotina para escrever o texto.
        StartCoroutine(TypeTextSequence());
    }

    private IEnumerator TypeTextSequence()
    {
        // Garante que o texto comece vazio.
        targetText.text = "";

        // Passa por cada parágrafo na lista.
        foreach (string paragraph in paragraphs)
        {
            // Digita o parágrafo letra por letra.
            foreach (char letter in paragraph.ToCharArray())
            {
                targetText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }
            // Adiciona uma quebra de linha entre os parágrafos, se houver mais de um.
            targetText.text += "\n";
        }

        Debug.Log("StaticTypewriter terminou de escrever.");
        // A corrotina termina aqui. O texto permanece na tela.
    }
}