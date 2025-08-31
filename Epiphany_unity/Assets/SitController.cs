using UnityEngine;
using UnityEngine.Events; // <<< PASSO 1: Adicione esta linha

public class SitController : MonoBehaviour, IInteractable
{
    [Header("Referências da Cena")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject playerSentadaAnimObject;

    // <<< PASSO 2: Crie o evento público >>>
    [Header("Eventos")]
    public UnityEvent OnPlayerSit;

    private bool isSitting = false;

    public void Interact()
    {
        if (isSitting) return;
        Sentar();
    }

    private void Sentar()
    {
        Debug.Log("Ação: Sentar.");
        playerObject.SetActive(false);
        playerSentadaAnimObject.SetActive(true);
        isSitting = true;

        // <<< PASSO 3: "Grite" o aviso! >>>
        OnPlayerSit.Invoke();
    }

    public void Levantar() // Este método continua aqui para o StandUpOnMove
    {
        Debug.Log("Ação: Levantar.");
        playerObject.SetActive(true);
        playerObject.transform.position = this.transform.position;
        playerSentadaAnimObject.SetActive(false);
        isSitting = false;
    }
}