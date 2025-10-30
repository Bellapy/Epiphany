using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections; // <-- A CORREÇÃO ESTÁ AQUI

public class ArrivalSceneController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [Tooltip("O GameObject da player sentada na baleia (começa ativo).")]
    [SerializeField] private GameObject playerOnWhaleObject;
    [Tooltip("O GameObject do player controlável (começa inativo).")]
    [SerializeField] private GameObject playerControllableObject;
    [Tooltip("O ponto onde o jogador controlável deve aparecer.")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("O texto de UI que diz 'Aperte E para sair'.")]
    [SerializeField] private TextMeshProUGUI descentPromptText;

    [Header("Diálogo")]
    [SerializeField] private DialogueData bluArrivalDialogue;

    [Header("Configurações de Tempo")]
    [SerializeField] private float initialDelay = 1.0f;

    private PlayerInputActions playerInputActions;
    private bool canDescend = false;

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

    private void Start()
    {
        if (playerOnWhaleObject != null) playerOnWhaleObject.SetActive(true);
        if (playerControllableObject != null) playerControllableObject.SetActive(false);
        if (descentPromptText != null) descentPromptText.gameObject.SetActive(false);

        StartCoroutine(ArrivalSequence());
    }

    private IEnumerator ArrivalSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        if (DialogueManager.Instance != null && bluArrivalDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(bluArrivalDialogue);
            yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueBoxActive());
        }

        if (descentPromptText != null)
        {
            descentPromptText.gameObject.SetActive(true);
        }
        canDescend = true;
    }

    private void Update()
    {
        if (!canDescend) return;

        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        bool interactInput = playerInputActions.Player.Interact.WasPressedThisFrame();

        if (moveInput.magnitude > 0.1f || interactInput)
        {
            DescendFromWhale();
        }
    }

    private void DescendFromWhale()
    {
        canDescend = false;

        if (descentPromptText != null)
        {
            descentPromptText.gameObject.SetActive(false);
        }

        if (playerOnWhaleObject != null)
        {
            playerOnWhaleObject.SetActive(false);
        }

        if (playerControllableObject != null)
        {
            if (spawnPoint != null)
            {
                playerControllableObject.transform.position = spawnPoint.position;
            }
            playerControllableObject.SetActive(true);
        }

        this.enabled = false;
    }
}