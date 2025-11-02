using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class FinalChoiceController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [SerializeField] private DialogueData aylaMonologue;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private CanvasGroup finalChoicePanel;
    [SerializeField] private Button sacrificeButton;
    [SerializeField] private Button noSacrificeButton;

    private bool hasBeenTriggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (finalChoicePanel != null) finalChoicePanel.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        hasBeenTriggered = true;
        StartCoroutine(FinalSceneSequence());
    }

    private IEnumerator FinalSceneSequence()
    {
        // 1. Desabilita o movimento do jogador.
        FindFirstObjectByType<PlayerController>()?.DisableMovement();

        // 2. Inicia o monólogo de Ayla e espera ele terminar.
        if (DialogueManager.Instance != null && aylaMonologue != null)
        {
            DialogueManager.Instance.StartDialogue(aylaMonologue);
            yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        }

        // 3. Espera 2 segundos.
        yield return new WaitForSeconds(2.0f);

        // 4. Inicia o fade-out para preto. A função de callback será chamada quando o fade terminar.
        if (fadeController != null)
        {
            fadeController.StartFadeOut(() => {
                StartCoroutine(ShowChoiceButtons());
            });
        }
    }

    private IEnumerator ShowChoiceButtons()
    {
        // Esta corrotina é chamada após a tela ficar preta.
        if (finalChoicePanel == null) yield break;

        finalChoicePanel.gameObject.SetActive(true);
        
        // Conecta os botões às suas funções.
        sacrificeButton.onClick.AddListener(OnSacrificeClicked);
        noSacrificeButton.onClick.AddListener(OnNotSacrificeClicked);

        // Fade-in dos botões.
        float timer = 0f;
        while (timer < 1.0f) // Duração do fade-in dos botões
        {
            timer += Time.deltaTime;
            finalChoicePanel.alpha = Mathf.Lerp(0, 1, timer / 1.0f);
            yield return null;
        }
        finalChoicePanel.alpha = 1;
        finalChoicePanel.interactable = true;
        finalChoicePanel.blocksRaycasts = true;
    }

    private void OnSacrificeClicked()
    {
        // Desativa a interatividade para evitar cliques duplos.
        finalChoicePanel.interactable = false;
        GameManager.Instance.LoadScene("sacrificou");
    }

    private void OnNotSacrificeClicked()
    {
        finalChoicePanel.interactable = false;
        GameManager.Instance.LoadScene("naosacrificou");
    }
}