using UnityEngine;
using UnityEngine.Events;

// A classe começa aqui
public class SitController : MonoBehaviour, IInteractable
{ // <--- Abertura da classe

    [Header("Referências da Cena")]
    [Tooltip("Arraste o GameObject da sua Player principal aqui.")]
    [SerializeField] private GameObject playerObject;
    
    [Tooltip("Arraste o GameObject com a animação da Player sentada aqui.")]
    [SerializeField] private GameObject playerSentadaAnimObject;

    [Header("Eventos")]
    [Tooltip("Este evento é disparado quando o jogador senta.")]
    public UnityEvent OnPlayerSit;

    private bool isPlayerSitting = false;

    /// <summary>
    /// Este método é chamado pelo PlayerInteractor quando o jogador aperta "E".
    /// </summary>
    public void Interact()
    {
        if (isPlayerSitting || playerObject == null || playerSentadaAnimObject == null) return;
        
        Sentar();
    }

    private void Sentar()
    {
        Debug.Log("Ação: Sentar.");
        playerObject.SetActive(false);
        playerSentadaAnimObject.SetActive(true);
        isPlayerSitting = true;
        OnPlayerSit.Invoke();
    }

    /// <summary>
    /// Este método PÚBLICO é chamado pelo script StandUpOnMove quando o jogador tenta se mover.
    /// </summary>
    public void Levantar()
    {
        if (!isPlayerSitting) return;

        Debug.Log("Ação: Levantar.");
        playerObject.SetActive(true);
        playerObject.transform.position = this.transform.position;
        playerSentadaAnimObject.SetActive(false);
        isPlayerSitting = false;
    }

} // <--- Fechamento da classe. Todo o código acima deve estar antes desta chave.