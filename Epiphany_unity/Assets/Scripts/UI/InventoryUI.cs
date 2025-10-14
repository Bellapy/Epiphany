using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<InventorySlotUI> uiSlots;

    private bool isInventoryOpen = false;

    // Awake é executado antes de Start, garantindo que o registro aconteça o mais cedo possível.
    void Awake()
    {
        // Garante que o inventário comece fechado
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        // A UI se apresenta/registra no Manager.
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RegisterUI(this);
        }
        else
        {
            // Este erro pode aparecer se o InventoryManager for criado depois da UI,
            // mas com a configuração de Singleton, é improvável.
            Debug.LogError("[InventoryUI] Não foi possível encontrar o InventoryManager para se registrar!");
        }
    }

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += UpdateUI;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= UpdateUI;
    }

    // Este método é público para ser chamado externamente (pelo Manager).
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            UpdateUI();
        }
        
        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }

    private void UpdateUI()
    {
        if (!isInventoryOpen) return; // Otimização: não desenha a UI se ela não estiver visível.

        List<InventorySlot> inventoryData = InventoryManager.Instance.GetInventorySlots();

        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < inventoryData.Count)
            {
                uiSlots[i].DrawSlot(inventoryData[i]);
            }
            else
            {
                uiSlots[i].ClearSlot();
            }
        }
    }
}