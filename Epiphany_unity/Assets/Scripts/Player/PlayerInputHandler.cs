using UnityEngine;

// Este script fica no mesmo GameObject que o Player Input.
public class PlayerInputHandler : MonoBehaviour
{
    // O Player Input vai chamar este método automaticamente por causa do nome.
    // O nome DEVE ser "On" + "NomeDaAção" (OpenInventory).
    public void OnOpenInventory()
    {
        // Verifica se o InventoryManager existe antes de chamá-lo.
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ToggleInventoryUI();
        }
        else
        {
            Debug.LogWarning("[PlayerInputHandler] Tentou abrir o inventário, mas o InventoryManager.Instance não foi encontrado!");
        }
    }
}