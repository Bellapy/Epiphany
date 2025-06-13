using UnityEngine;

// Garante que este GameObject sempre tenha um BoxCollider2D
[RequireComponent(typeof(BoxCollider2D))]
public class ReflectionTrigger : MonoBehaviour
{
    [Header("Dados da Reflexão")]
    [SerializeField] private ReflectionData reflectionData; // O pacote de dados da frase a ser exibida

    [Header("Configurações do Gatilho")]
    [SerializeField] private bool triggerOnce = true; // Se o gatilho deve ser ativado apenas uma vez

    private bool hasBeenTriggered = false; // Guarda se já foi ativado

    private void Awake()
    {
        // Garante que o collider seja um trigger
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // Este método é chamado automaticamente pela Unity quando outro collider entra neste trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou é o jogador e se o gatilho ainda não foi ativado
        if (other.CompareTag("Player") && (!triggerOnce || !hasBeenTriggered))
        {
            // Verifica se temos dados de reflexão para exibir
            if (reflectionData == null)
            {
                Debug.LogWarning($"ReflectionTrigger em '{gameObject.name}' não tem ReflectionData associado.");
                return;
            }

            // Avisa o UIManager para mostrar a reflexão
            UIManager.Instance.ShowReflection(reflectionData);
            
            // Marca que já foi ativado
            hasBeenTriggered = true;
        }
    }
}
