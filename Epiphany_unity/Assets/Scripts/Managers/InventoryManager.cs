using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public static event Action OnInventoryChanged;

    // Referência privada para a UI, que será preenchida pela própria UI.
    private InventoryUI uiInstance;

    [Header("Configuração do Inventário")]
    [SerializeField] private int maxSlots = 8;
    
    private List<InventorySlot> slots = new List<InventorySlot>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Método para a UI se registrar no Manager.
    public void RegisterUI(InventoryUI ui)
    {
        uiInstance = ui;
        Debug.Log("[InventoryManager] InventoryUI registrada com sucesso!");
    }

    public bool AddItem(ItemData itemToAdd)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToAdd)
            {
                slot.quantity++;
                OnInventoryChanged?.Invoke();
                Debug.Log($"Adicionado +1 '{itemToAdd.itemName}'. Total: {slot.quantity}");
                return true;
            }
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(itemToAdd, 1));
            OnInventoryChanged?.Invoke();
            Debug.Log($"Adicionado novo item '{itemToAdd.itemName}' ao inventário.");
            return true;
        }

        Debug.LogWarning($"Inventário cheio! Não foi possível adicionar '{itemToAdd.itemName}'.");
        return false;
    }
    
    public List<InventorySlot> GetInventorySlots()
    {
        return slots;
    }

    // Método ponte que agora usa a referência registrada.
    public void ToggleInventoryUI()
    {
        if (uiInstance != null)
        {
            uiInstance.ToggleInventory();
        }
        else
        {
            Debug.LogError("[InventoryManager] Tentou abrir a UI, mas nenhuma InventoryUI se registrou!");
        }
    }
}