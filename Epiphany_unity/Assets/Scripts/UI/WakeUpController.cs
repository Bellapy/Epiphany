using UnityEngine;
using UnityEngine.InputSystem; // Necessário para ler o novo Input System

public class WakeUpController : MonoBehaviour
{
    [Header("Referências de Personagem")]
    [Tooltip("Arraste o GameObject do Player controlável (que começa desativado) aqui.")]
    [SerializeField] private GameObject playerObject;

    [Tooltip("Arraste o GameObject que representa o Player deitado (que começa ativado) aqui.")]
    [SerializeField] private GameObject playerLyingDownObject;

    private PlayerInputActions playerInputActions;
    private bool hasWokenUp = false;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    void Start()
    {
        // Garante o estado inicial correto
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }
        if (playerLyingDownObject != null)
        {
            playerLyingDownObject.SetActive(true);
        }
    }

    void Update()
    {
        // Se já acordou, não faz mais nada.
        if (hasWokenUp) return;

        // Lê o valor do input de movimento (WASD ou Setas)
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Verifica se o jogador tentou se mover OU apertou a tecla de interação
        if (moveInput.magnitude > 0.1f || playerInputActions.Player.Interact.WasPressedThisFrame())
        {
            WakeUp();
        }
    }

    private void WakeUp()
    {
        hasWokenUp = true;
        Debug.Log("Jogador acordou!");

        // Faz a troca: ativa o jogador controlável
        if (playerObject != null)
        {
            playerObject.SetActive(true);
        }

        // Desativa a representação deitada
        if (playerLyingDownObject != null)
        {
            playerLyingDownObject.SetActive(false);
        }

        // Desativa este script para que o Update() pare de rodar
        this.enabled = false;
    }
}