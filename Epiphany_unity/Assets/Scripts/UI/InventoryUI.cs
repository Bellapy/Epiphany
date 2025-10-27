using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryUI : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<InventorySlotUI> uiSlots;

    private bool isInventoryOpen = false;

    void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateUI;
        }
        
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateUI;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

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
        if (uiSlots == null || InventoryManager.Instance == null) return;

        List<InventorySlot> inventoryData = InventoryManager.Instance.GetInventorySlots();

        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < uiSlots.Count && uiSlots[i] != null)
            {
                if (i < inventoryData.Count && inventoryData[i] != null)
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
}