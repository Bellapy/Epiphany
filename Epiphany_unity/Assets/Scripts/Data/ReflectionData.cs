using UnityEngine;
using System.Collections.Generic;

// Atributo que cria a opção no menu "Create" da Unity
[CreateAssetMenu(fileName = "NewReflection", menuName = "Epiphany/Reflection Data")]
public class ReflectionData : ScriptableObject
{
    [Header("Frases da Reflexão")]
    [TextArea(3, 10)] // Torna o campo de texto maior no Inspector
    public List<string> reflectionLines; // Lista de frases para a reflexão
}