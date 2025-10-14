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

   
    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("Target Text não foi atribuído no StaticTypewriter!", this.gameObject);
            return;
        }

    
        StartCoroutine(TypeTextSequence());
    }

    private IEnumerator TypeTextSequence()
    {
        
        targetText.text = "";

        
        foreach (string paragraph in paragraphs)
        {
           
            foreach (char letter in paragraph.ToCharArray())
            {
                targetText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }
            
            targetText.text += "\n";
        }

        Debug.Log("StaticTypewriter terminou de escrever.");

    }
}