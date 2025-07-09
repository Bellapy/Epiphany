using UnityEngine;

public class DialogueActivationZone : MonoBehaviour
{
    [Header("Dados")]
    [Tooltip("Arraste aqui o ASSET de diálogo que deve começar.")]
    [SerializeField] private DialogueData dialogueToStart;
    
    [Header("Gatilho de Evento")]
    [Tooltip("Arraste aqui o Manager do puzzle que será ativado após o diálogo.")]
    [SerializeField] private CrystalPuzzleManager puzzleManager;

    [Tooltip("Marque se este gatilho deve funcionar apenas uma vez.")]
    [SerializeField] private bool triggerOnce = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou na zona. Entregando diálogo e ação final para o DialogueManager.");
            DialogueManager.Instance.StartDialogue(dialogueToStart, puzzleManager.ActivatePuzzle);

            if (triggerOnce)
            {
                gameObject.SetActive(false);
            }
        }
    }
}