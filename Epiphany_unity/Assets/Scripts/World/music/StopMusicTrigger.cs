using UnityEngine;

// Este script para a música de fundo com um fade out suave
// quando o jogador entra em sua área de trigger.
public class StopMusicTrigger : MonoBehaviour
{
    [Header("Configuração do Fade")]
    [Tooltip("Duração do fade out da música ao entrar na área.")]
    [SerializeField] private float fadeOutDuration = 2.0f; // Duração de 2 segundos por padrão

    [Tooltip("Marque se este gatilho deve ser destruído após o primeiro uso.")]
    [SerializeField] private bool destroyOnTrigger = true;

    // A função OnTriggerEnter é chamada pela Unity automaticamente
    // quando um outro objeto com um Rigidbody entra neste trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos se quem entrou no trigger é o jogador (pela tag "Player")
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou no StopMusicTrigger. Iniciando fade out da música.");

            // Chamamos o nosso AudioManager para fazer a mágica do fade out.
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopMusicWithFade(fadeOutDuration);
            }

            // Se a opção for marcada, o objeto do trigger se autodestrói
            // para não ser ativado novamente.
            if (destroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
