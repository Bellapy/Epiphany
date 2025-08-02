using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CrystalPuzzleManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private List<PuzzleCrystal> crystals;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject barrier;

    [Header("Configuração do Puzzle")]
    [SerializeField] private List<int> correctSequence;
    [SerializeField] private float timeBetweenNotes = 0.7f;
    
    [Header("Configuração de Áudio Pós-Puzzle")]
    [Tooltip("A música que deve voltar a tocar quando o puzzle for resolvido.")]
    [SerializeField] private AudioClip sceneBackgroundMusic;

    private List<int> playerSequence = new List<int>();
    private int _selectedIndex = 0;
    private bool isPuzzleActive = false;
    private PlayerInputActions playerInputActions;
    private Coroutine melodyCoroutine;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.PuzzleUI.MoveSelection.performed += OnMoveSelection;
        playerInputActions.PuzzleUI.Activate.performed += OnActivateCrystal;
    }

    private void OnDisable()
    {
        playerInputActions.PuzzleUI.Disable();
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
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
            if (playerSequence.Count > correctSequence.Count || playerSequence[i] != correctSequence[i])
            {
                playerSequence.Clear();
                return;
            }
        }
        
        if (playerSequence.Count == correctSequence.Count)
        {
            SolvePuzzle();
        }
    }

    public void PlaySolutionSequence()
    {
        StopAllCoroutines();
        melodyCoroutine = StartCoroutine(PlaySequenceRoutine());
    }

    private IEnumerator PlaySequenceRoutine()
    {
        foreach (int crystalIndex in correctSequence)
        {
            if (crystalIndex >= 0 && crystalIndex < crystals.Count)
            {
                crystals[crystalIndex].ActivateCrystal();
                yield return new WaitForSeconds(timeBetweenNotes);
            }
        }
        melodyCoroutine = null;
    }

    private void SolvePuzzle()
    {
        // 1. Para imediatamente qualquer melodia que esteja tocando.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSFX();
        }
        
        // 2. Para a corrotina de loop de melodia, por segurança.
        StopAllCoroutines();

        // 3. Habilita o movimento do jogador e desativa o input do puzzle.
        if (playerController != null) playerController.EnableMovement();
        isPuzzleActive = false;
        playerInputActions.PuzzleUI.Disable();
        
        // 4. Faz a barreira desaparecer.
        if (barrier != null)
        {
            Animator barrierAnimator = barrier.GetComponent<Animator>();
            if (barrierAnimator != null) { barrierAnimator.SetTrigger("Disappear"); }
            else { barrier.SetActive(false); }
        }

        // 5. Reinicia a música de fundo da cena.
        if (AudioManager.Instance != null && sceneBackgroundMusic != null)
        {
            AudioManager.Instance.PlayMusicWithFade(sceneBackgroundMusic, 2.0f);
        }
    }
    
    public void ActivatePuzzle()
    {
        if (playerController != null) playerController.DisableMovement();
        isPuzzleActive = true;
        playerInputActions.PuzzleUI.Enable();
        playerSequence.Clear();
        _selectedIndex = 0;
        foreach (var crystal in crystals) { crystal.OnDeselected(); }
        crystals[_selectedIndex].OnSelected();
    }
    public void StopMelodyPlayback()
{
    Debug.Log("[CrystalPuzzleManager] Parando todas as corrotinas e SFX de melodia.");
    StopAllCoroutines();
    if (AudioManager.Instance != null)
    {
        // Esta função vai parar todos os sons temporários que criamos para as notas
        AudioManager.Instance.StopAllSFX(); 
    }
    melodyCoroutine = null;
}

    public float GetSequenceDuration()
    {
        if (correctSequence == null) return 0f;
        return correctSequence.Count * timeBetweenNotes;
    }
}