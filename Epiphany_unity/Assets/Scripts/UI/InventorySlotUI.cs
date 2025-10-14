using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Referências do Slot")]
    [Tooltip("A imagem que exibirá o ícone do item.")]
    [SerializeField] private Image iconImage;
    [Tooltip("O texto que exibirá a quantidade do item.")]
    [SerializeField] private TextMeshProUGUI quantityText;

    // Limpa o slot, deixando-o com aparência de vazio
    public void ClearSlot()
    {
        if (iconImage != null)
        {
            iconImage.enabled = false;
        }
        if (quantityText != null)
        {
            quantityText.enabled = false;
        }
    }

    // Preenche o slot com os dados de um item
    public void DrawSlot(InventorySlot slotData)
    {
        if (iconImage == null || slotData == null || slotData.item == null) return;

        iconImage.enabled = true;
        iconImage.sprite = slotData.item.icon;

        // Mostra a quantidade apenas se for maior que 1
        if (quantityText != null)
        {
            if (slotData.quantity > 1)
            {
                quantityText.enabled = true;
                quantityText.text = slotData.quantity.ToString();
            }
            else
            {
                quantityText.enabled = false;
            }
        }
    }
}