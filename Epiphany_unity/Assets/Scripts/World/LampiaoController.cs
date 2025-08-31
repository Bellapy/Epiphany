using UnityEngine;

public class LampiaoController : MonoBehaviour, IInteractable
{
    [Header("Referências Visuais")]
    [SerializeField] private GameObject spriteAceso;
    [SerializeField] private GameObject spriteApagado;
    [SerializeField] private int lampiaoID;

    private bool isAceso = false;

    public void Interact()
    {
        ToggleLuz();
    }

    public void ToggleLuz()
    {
        isAceso = !isAceso;
        spriteAceso.SetActive(isAceso);
        spriteApagado.SetActive(!isAceso);
    }

    public void ResetarLampiao()
    {
        isAceso = false;
        spriteAceso.SetActive(false);
        spriteApagado.SetActive(true);
    }

    public bool EstaAceso()
    {
        return isAceso;
    }

    public int GetID()
    {
        return lampiaoID;
    }
}