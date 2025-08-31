using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReflectionSign : MonoBehaviour, IInteractable
{
    [Header("Dados da Reflexão")]
    [SerializeField] private ReflectionData reflectionData;

    [Header("Configuração de Tempo")]
    [Tooltip("Quantos segundos a mensagem fica visível ANTES de começar a desaparecer.")]
    [SerializeField] private float displayDuration = 2.0f;
    
    [Tooltip("Quanto tempo o efeito de fade out (desaparecer) leva.")]
    [SerializeField] private float fadeDuration = 1.0f;

    public void Interact()
    {
        if (reflectionData != null && DialogueManager.Instance != null)
        {
            Debug.Log($"Placa '{gameObject.name}' interagida. Chamando reflexão com fade out.");
            
            // Agora chamamos um novo método, passando os dois tempos.
            DialogueManager.Instance.StartReflectionWithFadeOut(reflectionData, displayDuration, fadeDuration);
        }
        else
        {
            Debug.LogWarning($"Placa '{gameObject.name}' não tem ReflectionData ou DialogueManager não foi encontrado.");
        }
    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}