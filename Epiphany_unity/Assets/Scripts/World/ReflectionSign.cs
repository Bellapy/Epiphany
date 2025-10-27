using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReflectionSign : MonoBehaviour, IInteractable
{
    [Header("Dados da Reflexão")]
    [Tooltip("Arraste o ScriptableObject com o texto da reflexão aqui.")]
    [SerializeField] private ReflectionData reflectionData;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact()
    {
        if (reflectionData != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartReflection(reflectionData);
        }
    }
}