using UnityEngine;
using UnityEngine.Events;

public class ChairController : MonoBehaviour, IInteractable
{
    [Header("Referências")]
    [Tooltip("Arraste o GameObject que representa a player sentada aqui.")]
    [SerializeField] private GameObject playerSittingObject;
    [Tooltip("Ponto exato onde o jogador original deve reaparecer ao levantar.")]
    [SerializeField] private Transform standUpPoint;

    [Header("Eventos")]
    public UnityEvent OnPlayerSit;
    public UnityEvent OnPlayerStandUp;

    private PlayerController playerController;
    private bool isPlayerSitting = false;

    public void Interact()
    {
        if (!isPlayerSitting)
        {
            SitDown();
        }
        else
        {
            StandUp();
        }
    }

    private void SitDown()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null) return;

        isPlayerSitting = true;
        
        // Desativa o jogador controlável
        playerController.gameObject.SetActive(false);
        
        // Ativa o objeto com a animação de sentar
        if (playerSittingObject != null)
        {
            playerSittingObject.SetActive(true);
        }

        OnPlayerSit.Invoke();
    }

    private void StandUp()
    {
        if (playerController == null) return;

        isPlayerSitting = false;

        // Desativa o objeto com a animação de sentar
        if (playerSittingObject != null)
        {
            playerSittingObject.SetActive(false);
        }

        // Reposiciona e reativa o jogador controlável
        if (standUpPoint != null)
        {
            playerController.transform.position = standUpPoint.position;
        }
        playerController.gameObject.SetActive(true);
        // O jogador já estará com movimento habilitado por padrão ao ser reativado.

        OnPlayerStandUp.Invoke();
    }
}