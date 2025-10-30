using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SpiritEncounterController : MonoBehaviour
{
    [Header("Gerenciamento de Estado")]
    [Tooltip("Nome da flag que marca este evento como concluído.")]
    [SerializeField] private string completionFlag = "FloresSpiritEncounterCompleted";

    [Header("Referências do Evento")]
    [SerializeField] private SpriteRenderer spiritSpriteRenderer;
    [SerializeField] private DialogueData spiritDialogue;

    [Header("Configurações de Tempo")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float postDialogueWait = 2.0f;
    [SerializeField] private float fadeOutDuration = 2.0f;

    private bool hasBeenTriggered = false;
    private PlayerController playerController;

    private void Awake()
    {
        // --- LÓGICA DE PERSISTÊNCIA ADICIONADA ---
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            // Se o evento já aconteceu, desativa o espírito e o gatilho.
            if (spiritSpriteRenderer != null) spiritSpriteRenderer.gameObject.SetActive(false);
            gameObject.SetActive(false);
            return;
        }
        // --- FIM DA LÓGICA DE PERSISTÊNCIA ---

        GetComponent<Collider2D>().isTrigger = true;
        
        if (spiritSpriteRenderer != null)
        {
            var color = spiritSpriteRenderer.color;
            color.a = 0;
            spiritSpriteRenderer.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true;
        playerController = other.GetComponent<PlayerController>();
        StartCoroutine(EncounterSequence());
    }

    private IEnumerator EncounterSequence()
    {
        yield return StartCoroutine(FadeSprite(1f, fadeInDuration));

        if (playerController != null) playerController.DisableMovement();
        
        if (DialogueManager.Instance != null && spiritDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(spiritDialogue);
        }

        yield return new WaitForSeconds(postDialogueWait);

        // --- LÓGICA DE FADE SIMULTÂNEO ADICIONADA ---
        // Inicia o fade do espírito E o fade do painel de diálogo ao mesmo tempo.
        StartCoroutine(FadeSprite(0f, fadeOutDuration));
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartFadeOutDialogueBox(fadeOutDuration);
        }
        
        // Espera o tempo do fade terminar antes de continuar.
        yield return new WaitForSeconds(fadeOutDuration);
        // --- FIM DA LÓGICA DE FADE ---

        if (playerController != null) playerController.EnableMovement();

        // --- SALVANDO O ESTADO ---
        PlayerPrefs.SetInt(completionFlag, 1);
        PlayerPrefs.Save();
        // --- FIM DO SALVAMENTO ---

        gameObject.SetActive(false);
    }

    private IEnumerator FadeSprite(float targetAlpha, float duration)
    {
        if (spiritSpriteRenderer == null) yield break;

        float startAlpha = spiritSpriteRenderer.color.a;
        Color color = spiritSpriteRenderer.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            spiritSpriteRenderer.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        spiritSpriteRenderer.color = color;
    }
}