using UnityEngine;

// IInteractable é a "etiqueta" que diz ao PlayerInteractor que este objeto pode ser interagido.
// Sem isso, o jogador não saberia como "falar" com ele.
public class PuzzleCristalNPC : MonoBehaviour, IInteractable
{
    [Header("Diálogo")]
    [Tooltip("Arraste aqui o asset de diálogo que este NPC vai iniciar.")]
    [SerializeField] private DialogueData dialogueToStart;

    // Esta é a função que o PlayerInteractor chama quando apertamos "E".
    public void Interact()
    {
        // Vamos usar o padrão Singleton para acessar o DialogueManager.
        // Isso garante que ele funcione em qualquer cena.
        DialogueManager.Instance.StartDialogue(dialogueToStart);
        
        // Para garantir que o diálogo não seja interrompido ou reiniciado,
        // vamos desativar a capacidade de interagir novamente logo após começar.
        GetComponent<Collider2D>().enabled = false;
    }
}
