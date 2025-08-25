using UnityEngine;
using UnityEngine.Events; // Necessário para usar UnityEvent

// Este script implementa IInteractable para ser detectado pelo PlayerInteractor
public class LampiaoController : MonoBehaviour, IInteractable
{
    [Header("Referências Visuais")]
    [Tooltip("Arraste aqui o GameObject do sprite do lampião aceso.")]
    [SerializeField] private GameObject spriteAceso;
    
    [Tooltip("Arraste aqui o GameObject do sprite do lampião apagado.")]
    [SerializeField] private GameObject spriteApagado;

    [Header("Identificação do Puzzle")]
    [Tooltip("O ID único deste lampião (0, 1, 2, 3, etc.).")]
    [SerializeField] private int lampiaoID;

    // Um "evento" que vai avisar o gerente do puzzle quando este botão for pressionado.
    public UnityEvent<int> OnLampiaoPressed;

    private bool isAceso = false;

    public void Interact()
    {
        // Inverte o estado atual: se estava aceso, apaga; se estava apagado, acende.
        ToggleLuz();

        // Avisa o gerente do puzzle que este lampião foi pressionado, enviando seu ID.
        OnLampiaoPressed.Invoke(lampiaoID);
    }

    public void ToggleLuz()
    {
        isAceso = !isAceso;
        
        // Atualiza os sprites com base no novo estado.
        spriteAceso.SetActive(isAceso);
        spriteApagado.SetActive(!isAceso);
    }

    // Função pública para o gerente do puzzle poder resetar o lampião.
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

    // Nova função para o gerente poder pegar o ID.
    public int GetID()
    {
        return lampiaoID;
    }
}