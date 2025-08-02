using UnityEngine;

public class PuzzleCrystal : MonoBehaviour
{
    [Header("Feedback")]
    [Tooltip("O som que este cristal toca quando ativado.")]
    [SerializeField] private AudioClip crystalNote;
    
    [Tooltip("O Animator deste cristal.")]
    private Animator animator;

    private void Awake()
    {
        // Pega o componente Animator automaticamente.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"O cristal '{gameObject.name}' não tem um componente Animator!");
        }
    }

    /// <summary>
    /// Ativa o cristal, tocando seu som e sua animação de ativação.
    /// </summary>
    public void ActivateCrystal()
    {
        if (crystalNote != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(crystalNote);
        }

        if (animator != null)
        {
            // Usamos um Trigger para a animação de "brilho", pois é um evento único.
            animator.SetTrigger("Activate");
        }
    }

    /// <summary>
    /// Ativa o feedback visual de que este cristal está selecionado.
    /// </summary>
    public void OnSelected()
    {
        if (animator != null)
        {
            animator.SetBool("IsSelected", true);
        }
    }

    /// <summary>
    /// Desativa o feedback visual de seleção.
    /// </summary>
    public void OnDeselected()
    {
        if (animator != null)
        {
            animator.SetBool("IsSelected", false);
        }
    }
}