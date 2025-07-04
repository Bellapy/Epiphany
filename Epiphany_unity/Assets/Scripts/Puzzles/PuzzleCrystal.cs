using UnityEngine;

public class PuzzleCrystal : MonoBehaviour
{
    [SerializeField] private AudioClip crystalNote;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ActivateCrystal()
    {
        if (crystalNote != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(crystalNote);
        }
        if (animator != null)
        {
            animator.SetBool("IsActivated", true);
        }
    }

    public void OnSelected()
    {
        if (animator != null)
        {
            animator.SetBool("IsSelected", true);
        }
    }

/*************  ✨ Windsurf Command ⭐  *************/
/// <summary>
/// Deselects the crystal, resetting its visual states in the animator.
/// <para>Disables both the "IsSelected" and "IsActivated" states when the crystal is deselected.</para>
/// </summary>

/*******  017f8698-1d17-4bc4-99c4-87b16afcc3c6  *******/
    public void OnDeselected()
    {
        if (animator != null)
        {
            // Quando a seleção sai, desliga tanto o pulso quanto o estado ativado
            animator.SetBool("IsSelected", false);
            animator.SetBool("IsActivated", false);
        }
    }
}