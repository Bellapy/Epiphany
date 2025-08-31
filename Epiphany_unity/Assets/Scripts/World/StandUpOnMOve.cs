using UnityEngine;
using UnityEngine.InputSystem; // Necessário para ler o Input System

public class StandUpOnMove : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o GameObject da Player principal (que está desativado) aqui.")]
    [SerializeField] private GameObject playerObject;

    [Tooltip("Arraste o SitController (que está no Poltrona_Interativa) aqui.")]
    [SerializeField] private SitController sitController;

    private PlayerInputActions playerInputActions;

    // Awake é chamado quando este objeto é ativado
    private void Awake()
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

    // Update é chamado a cada frame
    void Update()
    {
        // Lê o valor do input de movimento (WASD ou Setas)
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Se o jogador tentou se mover em qualquer direção...
        if (moveInput.magnitude > 0.1f)
        {
            Debug.Log("Tentativa de movimento detectada! Levantando...");

            // ...chama a função pública para levantar no SitController.
            if (sitController != null)
            {
                sitController.Levantar();
            }
        }
    }
}
