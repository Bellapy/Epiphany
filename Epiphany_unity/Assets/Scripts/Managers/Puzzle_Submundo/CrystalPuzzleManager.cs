using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CrystalPuzzleManager : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste todos os GameObjects de cristal aqui, na ordem de seleção.")]
    [SerializeField] private List<PuzzleCrystal> crystals;
    
    // <<< NOVO CAMPO PARA CONECTAR NO INSPECTOR >>>
    [Tooltip("Arraste o GameObject da Player que contém o PlayerController.")]
    [SerializeField] private PlayerController playerController;

    [Header("Configuração do Puzzle")]
    [Tooltip("A sequência correta a ser tocada. Use os números de 0 a (n-1), baseados na ordem da lista 'crystals'.")]
    [SerializeField] private List<int> correctSequence;
    [Tooltip("Tempo de espera em segundos entre cada nota da melodia.")]
    [SerializeField] private float timeBetweenNotes = 0.7f;
    
    [Header("Controle do Puzzle")]
    [Tooltip("Arraste aqui o GameObject da barreira que será desativado.")]
    [SerializeField] private GameObject barrier;
    
    private List<int> playerSequence = new List<int>();
    private int _selectedIndex = 0;
    private bool isPuzzleActive = false;
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.PuzzleUI.MoveSelection.performed += OnMoveSelection;
        playerInputActions.PuzzleUI.Activate.performed += OnActivateCrystal;
    }

    private void OnEnable()
    {
        // ATIVA O CONTROLE DOS CRISTAIS
        playerInputActions.PuzzleUI.Enable();
    }

    private void OnDisable()
    {
        // DESATIVA O CONTROLE DOS CRISTAIS (BOA PRÁTICA)
        playerInputActions.PuzzleUI.Disable();
    }

    private void OnMoveSelection(InputAction.CallbackContext context)
    {
        if (!isPuzzleActive) return;
        float moveDirection = context.ReadValue<float>();
        if (moveDirection > 0.5f) { MoveSelection(1); }
        else if (moveDirection < -0.5f) { MoveSelection(-1); }
    }
    
    private void OnActivateCrystal(InputAction.CallbackContext context)
    {
        if (!isPuzzleActive) return;
        crystals[_selectedIndex].ActivateCrystal();
        playerSequence.Add(_selectedIndex);
        CheckPlayerSequence();
    }

    private void MoveSelection(int direction)
    {
        crystals[_selectedIndex].OnDeselected();
        _selectedIndex += direction;
        if (_selectedIndex >= crystals.Count) _selectedIndex = 0;
        else if (_selectedIndex < 0) _selectedIndex = crystals.Count - 1;
        crystals[_selectedIndex].OnSelected();
    }
    
    private void CheckPlayerSequence()
    {
        for (int i = 0; i < playerSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                Debug.Log("Sequência errada! Tentando de novo.");
                playerSequence.Clear();
                return;
            }
        }
        
        if (playerSequence.Count == correctSequence.Count)
        {
            Debug.Log("SEQUÊNCIA CORRETA! Puzzle resolvido!");
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        // <<< AQUI DEVOLVEMOS O CONTROLE PARA A AYLA >>>
        if (playerController != null)
        {
            playerController.EnableMovement();
        }

        isPuzzleActive = false;
        
        if (barrier != null)
        {
            barrier.SetActive(false);
        }

        Debug.Log("A barreira sumiu! Caminho livre.");
        
        // Opcional: Desativar este manager para não poder ser usado de novo.
        // gameObject.SetActive(false);
    }
    
    public void ActivatePuzzle()
    {
        // <<< AQUI CONGELAMOS A AYLA >>>
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        isPuzzleActive = true;
        playerSequence.Clear();
        _selectedIndex = 0;
        foreach (var crystal in crystals) { crystal.OnDeselected(); }
        crystals[_selectedIndex].OnSelected();
    }
}