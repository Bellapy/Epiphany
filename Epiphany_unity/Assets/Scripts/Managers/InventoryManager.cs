using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public event Action OnInventoryChanged;

    [Header("Configuração do Inventário")]
    [SerializeField] private int maxSlots = 8;
    
    private List<InventorySlot> slots = new List<InventorySlot>();

    public bool AddItem(ItemData itemToAdd)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToAdd)
            {
                slot.quantity++;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(itemToAdd, 1));
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.LogWarning($"Inventário cheio! Não foi possível adicionar '{itemToAdd.itemName}'.");
        return false;
    }

    public List<InventorySlot> GetInventorySlots()
    {
        return slots;
    }
}