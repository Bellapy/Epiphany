using UnityEngine;
using UnityEngine.InputSystem; // <-- IMPORTANTE: Adiciona a biblioteca do Input System
using System.Collections;
using System.Collections.Generic;

public class CrystalPuzzleManager : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste todos os GameObjects de cristal aqui, na ordem de seleção.")]
    [SerializeField] private List<PuzzleCrystal> crystals;

    [Header("Configuração do Puzzle")]
    [Tooltip("A sequência correta a ser tocada. Use os números de 0 a (n-1), baseados na ordem da lista 'crystals'.")]
    [SerializeField] private List<int> correctSequence;

    [Tooltip("Tempo de espera em segundos entre cada nota da melodia.")]
    [SerializeField] private float timeBetweenNotes = 0.7f;
    
    private int _selectedIndex = 0;
    private bool isPuzzleActive = false;
    
    private PlayerInputActions playerInputActions; // <-- NOVO: Referência para o nosso asset de ações

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        
        // "Assina" os eventos do mapa de ações "PuzzleUI".
        playerInputActions.PuzzleUI.MoveSelection.performed += OnMoveSelection;
        playerInputActions.PuzzleUI.Activate.performed += OnActivate;
    }

    // ADICIONE ESTAS DUAS FUNÇÕES
    private void OnEnable()
    {
        playerInputActions.PuzzleUI.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PuzzleUI.Disable();
    }

    private void OnMoveSelection(InputAction.CallbackContext context)
    {
        if (!isPuzzleActive) return;

       // LINHA CORRIGIDA
        float moveDirection = context.ReadValue<float>();
        
        if (moveDirection > 0.5f) { MoveSelection(1); } // Direita
        else if (moveDirection < -0.5f) { MoveSelection(-1); } // Esquerda
    }

    private void OnActivate(InputAction.CallbackContext context)
    {
        if (!isPuzzleActive) return;
        
        crystals[_selectedIndex].ActivateCrystal();
    }

    private void MoveSelection(int direction)
    {
        crystals[_selectedIndex].OnDeselected();
        _selectedIndex += direction;

        if (_selectedIndex >= crystals.Count) _selectedIndex = 0;
        else if (_selectedIndex < 0) _selectedIndex = crystals.Count - 1;
        
        crystals[_selectedIndex].OnSelected();
    }
    
    // --- Funções Públicas para Controle Externo ---
    public void ActivatePuzzle()
    {
        isPuzzleActive = true;
        //playerInputActions.PuzzleUI.Enable(); // Habilita o mapa de ações do puzzle
        
        _selectedIndex = 0;
        foreach (var crystal in crystals) { crystal.OnDeselected(); }
        crystals[_selectedIndex].OnSelected();
    }

    public void DeactivatePuzzle()
    {
        isPuzzleActive = false;
        //playerInputActions.PuzzleUI.Disable(); // Desabilita o mapa de ações do puzzle
        
        foreach (var crystal in crystals) { crystal.OnDeselected(); }
    }
    
    // ... (o resto das funções, como PlaySolutionSequence, continua igual) ...
    public void PlaySolutionSequence()
    {
        if (isPuzzleActive)
        {
            StartCoroutine(PlaySequenceRoutine());
        }
    }

    private IEnumerator PlaySequenceRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        foreach (int crystalIndex in correctSequence)
        {
            if (crystalIndex >= 0 && crystalIndex < crystals.Count)
            {
                // LINHA CORRIGIDA
                crystals[crystalIndex].ActivateCrystal();
                yield return new WaitForSeconds(timeBetweenNotes);
            }
        }
    }
}