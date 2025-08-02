using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Epiphany/Dialogue Data")]
public class DialogueData : ScriptableObject 
{
    [Header("Informações do Diálogo")]
    public string speakerName;
    public Sprite speakerPortrait;

    public List<DialogueLine> dialogueLines;

    [Header("Fim do Diálogo")]
    [Tooltip("Se marcado, mostrará opções de escolha no final.")]
    public bool hasChoice;

    // <<< ADICIONE ESTA LINHA >>>
    [Tooltip("A frase que aparece JUNTO com os botões de escolha.")]
    [TextArea(2, 5)]
    public string choicePrompt;

    public List<ChoiceOption> choiceOptions;
    
    // <<< LINHA RE-ADICIONADA AQUI >>>
    [Tooltip("Opcional: Diálogo a ser iniciado automaticamente quando este terminar (ignorado se 'hasChoice' estiver marcado).")]
    public DialogueData nextDialogueOnEnd;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string sentence;
    public bool triggerEventAfterLine;
    public UnityEvent onLineCompleteEvent;
}

[System.Serializable]
public class ChoiceOption
{
    public string optionText;
    public DialogueData nextDialogue;
}