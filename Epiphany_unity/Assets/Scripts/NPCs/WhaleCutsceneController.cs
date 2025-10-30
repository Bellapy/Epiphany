using UnityEngine;
using System.Collections;

public class WhaleCutsceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [Tooltip("O diálogo completo de Blu para esta cena.")]
    [SerializeField] private DialogueData bluMonologue;
    [Tooltip("O nome exato da próxima cena a ser carregada.")]
    [SerializeField] private string nextSceneName = "arvoredavida1";
    
    [Header("Configurações de Tempo")]
    [SerializeField] private float initialDelay = 3.0f;
    [SerializeField] private float finalDelay = 3.0f;

    private FadeController fadeController;

    private void Start()
    {
        // Encontra o FadeController na cena (provavelmente no Canvas persistente).
        fadeController = FindFirstObjectByType<FadeController>();
        
        // Inicia a sequência principal da cutscene.
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // 1. Espera inicial de 3 segundos.
        yield return new WaitForSeconds(initialDelay);

        // 2. Inicia o diálogo e espera ele terminar.
        if (DialogueManager.Instance != null && bluMonologue != null)
        {
            DialogueManager.Instance.StartDialogue(bluMonologue);
            // Espera até que o DialogueManager não esteja mais com a caixa de diálogo ativa.
            yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        }

        // 3. Espera final de 3 segundos.
        yield return new WaitForSeconds(finalDelay);

        // 4. Inicia a transição de cena.
        if (fadeController != null && GameManager.Instance != null)
        {
            fadeController.StartFadeOut(() => {
                GameManager.Instance.LoadScene(nextSceneName);
            });
        }
        else if (GameManager.Instance != null)
        {
            // Fallback caso o FadeController não seja encontrado.
            GameManager.Instance.LoadScene(nextSceneName);
        }
    }
}