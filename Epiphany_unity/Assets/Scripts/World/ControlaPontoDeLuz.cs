using UnityEngine;
using System.Collections;

public class ControlaPontoDeLuz : MonoBehaviour
{
    // Referência para o nosso gerenciador
    public LevelManager levelManager; // <<<<<<< ADICIONE ESTA LINHA

    public SpriteRenderer spriteAceso;
    public float duracaoDoFade = 2.0f;
    private bool jaFoiAtivado = false;

    void Start()
    {
        spriteAceso.color = new Color(spriteAceso.color.r, spriteAceso.color.g, spriteAceso.color.b, 0f);
    }

/*************  ✨ Windsurf Command ⭐  *************/
/// <summary>
/// Called when another object enters the trigger collider attached to this object.
/// Initiates the fade-in effect if the entering object is tagged as "Player"
/// and the effect has not been activated yet.
/// </summary>
/// <param name="other">The Collider2D object that enters the trigger.</param>

/*******  74062f18-f905-4832-8336-78e4d890db2c  *******/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !jaFoiAtivado)
        {
            jaFoiAtivado = true;
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float tempoPassado = 0f;
        Color corInicial = spriteAceso.color;
        Color corFinal = new Color(corInicial.r, corInicial.g, corInicial.b, 1f);

        while (tempoPassado < duracaoDoFade)
        {
            tempoPassado += Time.deltaTime;
            spriteAceso.color = Color.Lerp(corInicial, corFinal, tempoPassado / duracaoDoFade);
            yield return null;
        }

        spriteAceso.color = corFinal;

        // --- AVISANDO O GERENCIADOR ---
        // Depois de acender completamente, chama a função do LevelManager
        if (levelManager != null) // <<<<<<< ADICIONE ESTAS 3 LINHAS
        {
            levelManager.RegistrarCristalAceso();
        }
    }
}