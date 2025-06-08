using UnityEngine;
using System.Collections.Generic; // Necessário para usar List

// [CreateAssetMenu] permite que você crie assets deste tipo no menu 'Create' da Unity.
// fileName: Nome padrão do arquivo quando você cria um novo.
// menuName: O caminho no menu 'Create'. Ex: "Epiphany/Dialogue Data"
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Epiphany/Dialogue Data")]
public class DialogueData : ScriptableObject // IMPORTANTE: Herda de ScriptableObject, NÃO de MonoBehaviour
{
    // Campos para o bloco de fala
    public string speakerName; // Nome do personagem falando (opcional)
    public Sprite speakerPortrait; // Imagem do retrato do personagem

    // [TextArea] ajuda a ter um campo de texto maior no Inspector para as falas
    [TextArea(3, 10)] // 3 linhas mínimas, 10 máximas no Inspector
    public List<string> dialogueLines; // Lista de todas as falas nesta parte do diálogo

    // Campos para as "falsas escolhas"
    public bool hasChoice; // true se este diálogo apresentar uma "falsa escolha"
    public List<string> choiceOptions; // Opções de texto para a falsa escolha
    // Futuramente, você pode adicionar uma referência para o próximo DialogueData
    // que cada escolha leva, mas por enquanto, vamos manter simples.

    // Campos para as Reflexões Ambientais automáticas
    public bool isEnvironmentalReflection; // true se for uma reflexão automática
    public float displayDuration = 5.0f; // Duração que a reflexão fica na tela
}
