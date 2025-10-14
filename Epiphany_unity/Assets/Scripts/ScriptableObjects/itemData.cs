using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Epiphany/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Informações do Item")]
    public string itemName;
    public Sprite icon;
    [TextArea(3, 5)]
    public string description;
}