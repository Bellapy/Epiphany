using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Epiphany/Dialogue Data")]
public class DialogueData : ScriptableObject 
{
    public string speakerName;
    public Sprite speakerPortrait;

    [TextArea(3, 10)]
    public List<string> dialogueLines;

    public bool hasChoice;
    public List<string> choiceOptions;
    
    public bool isEnvironmentalReflection;
    public float displayDuration = 5.0f;
}