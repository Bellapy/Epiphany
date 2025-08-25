using UnityEngine;

// Este script agora é o "cérebro" da nossa escada automática
public class LadderZone : MonoBehaviour
{
    [Header("Configurações da Sequência")]
    [Tooltip("Ponto exato onde a personagem deve ficar antes de subir.")]
    [SerializeField] private Transform startClimbPoint;
    
    [Tooltip("Ponto final para onde a personagem se moverá ao subir.")]
    [SerializeField] private Transform endClimbPoint;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string spawnPointInNextScene;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Avisa o PlayerController que ele PODE subir e passa as referências
                player.SetCanClimb(true, this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Avisa o PlayerController que ele NÃO PODE mais subir
                player.SetCanClimb(false, null);
            }
        }
    }

    // Método público para o PlayerController poder pegar as informações
    public void StartAutomaticClimb(PlayerController player)
    {
        player.StartClimbingSequence(startClimbPoint, endClimbPoint, nextSceneName, spawnPointInNextScene);
    }
}