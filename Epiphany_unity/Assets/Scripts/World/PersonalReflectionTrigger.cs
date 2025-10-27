using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PersonalReflectionTrigger : MonoBehaviour, IInteractable
{
    [Header("Dados da Reflexão Pessoal")]
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