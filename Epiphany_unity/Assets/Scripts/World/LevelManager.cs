using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public GameObject raizesParaSumir;
    public int totalDeCristais = 3;
    private int cristaisAcesos = 0;

    public void RegistrarCristalAceso()
    {
        cristaisAcesos++;
        Debug.Log("Cristal registrado! Total de acesos agora: " + cristaisAcesos + " de " + totalDeCristais);

        if (cristaisAcesos >= totalDeCristais)
        {
            Debug.Log("CONDIÇÃO ATINGIDA! Tentando desaparecer com as raízes...");
            DesaparecerRaizes();
        }
    }

    private void DesaparecerRaizes()
    {
        if (raizesParaSumir == null)
        {
            Debug.LogError("ERRO: O objeto das raízes (raizesParaSumir) não foi definido no Inspector do LevelManager!");
            return;
        }
        StartCoroutine(FadeOutRaizes());
    }

    private IEnumerator FadeOutRaizes()
    {
        Debug.Log("Iniciando a corrotina FadeOutRaizes.");
        
        SpriteRenderer spriteDasRaizes = raizesParaSumir.GetComponent<SpriteRenderer>();
        if (spriteDasRaizes == null)
        {
            Debug.LogError("ERRO: O objeto definido em 'raizesParaSumir' não tem um componente SpriteRenderer!");
            raizesParaSumir.SetActive(false); // Plano B: some de uma vez
            yield break;
        }

        float duracaoDoFade = 2.5f;
        float tempoPassado = 0f;
        Color corBase = spriteDasRaizes.color;

        while (tempoPassado < duracaoDoFade)
        {
            tempoPassado += Time.deltaTime;
            float progresso = tempoPassado / duracaoDoFade;
            float novoAlpha = Mathf.Lerp(1f, 0f, progresso);
            spriteDasRaizes.color = new Color(corBase.r, corBase.g, corBase.b, novoAlpha);
            yield return null;
        }

        // Garante que o objeto seja desativado no final
        raizesParaSumir.SetActive(false);
        Debug.Log("Fade-out concluído. Raízes desativadas.");
    }
}