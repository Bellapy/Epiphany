using UnityEngine;

public class SpiritNPC : MonoBehaviour, IInteractable 
{
    [Header("Sequência de Diálogos")]
    public DialogueData introDialogue;
    public DialogueData postMelodyAndChoiceDialogue;

    // A função precisa existir para cumprir o contrato da interface IInteractable.
    public void Interact()
    {
        // Podemos deixar vazia ou colocar um log para testes.
        Debug.Log($"O objeto '{gameObject.name}' foi interagido, mas a lógica principal está no PuzzleSceneController.");
    }
}