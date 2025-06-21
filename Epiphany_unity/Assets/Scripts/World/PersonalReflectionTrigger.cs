// Em _Scripts/World/PersonalReflectionTrigger.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PersonalReflectionTrigger : MonoBehaviour, IInteractable
{
    [Header("Dados da Reflexão Pessoal")]
    [SerializeField] private ReflectionData reflectionData;

    private void Awake()
    {
        // Garante que o collider seja um gatilho para não bloquear o jogador.
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Este método é chamado pelo PlayerInteractor quando o jogador aperta "E"
    public void Interact()
    {
         Debug.Log("Tecla 'E' pressionada!");
        if (reflectionData != null && UIManager.Instance != null)
        {
            Debug.Log($"Gatilho pessoal '{gameObject.name}' interagido. Mostrando reflexão da personagem.");
            
            // <<< AQUI ESTÁ A CHAMADA CORRETA >>>
            // Ele chama o método que mostra o diálogo COM o retrato.
            UIManager.Instance.ShowPersonalReflection(reflectionData);
        }
        else
        {
            Debug.LogWarning($"Gatilho '{gameObject.name}' não tem ReflectionData ou UIManager não foi encontrado.");
        }
    }
}