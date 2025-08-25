using UnityEngine;
using System.Collections.Generic; // Necessário para Listas
using System.Linq; // Necessário para uma verificação mais fácil

public class PuzzleLuzesManager : MonoBehaviour
{
    [Header("Configuração do Puzzle")]
    [Tooltip("Defina os IDs dos lampiões que devem estar ACESOS para resolver o puzzle.")]
    [SerializeField] private List<int> idsCorretos = new List<int>();

    [Header("Referências da Cena")]
    [Tooltip("Arraste aqui a Porta que será destrancada.")]
    [SerializeField] private PortaPuzzle porta;
    
    [Tooltip("Arraste todos os scripts LampiaoController para esta lista.")]
    [SerializeField] private List<LampiaoController> lampioes;

    private bool puzzleResolvido = false;

    /// <summary>
    /// Este método público será chamado por cada lampião quando for pressionado.
    /// </summary>
    /// <param name="lampiaoID">O ID do lampião que foi pressionado.</param>
    public void VerificarEstadoDosLampioes(int lampiaoID)
    {
        // Se o puzzle já foi resolvido, não faz mais nada.
        if (puzzleResolvido) return;

        Debug.Log("Lampião " + lampiaoID + " pressionado. Verificando o estado de todos os lampiões...");

        // Cria uma lista temporária com os IDs de todos os lampiões que estão ACESOS AGORA.
        List<int> lampioesAcesosAtualmente = new List<int>();
        foreach (LampiaoController lampiao in lampioes)
        {
            if (lampiao.EstaAceso())
            {
                lampioesAcesosAtualmente.Add(lampiao.GetID());
            }
        }

        // Compara a lista de lampiões acesos com a lista de solução.
        // A ordem não importa, apenas se os elementos são os mesmos.
        bool estadoCorreto = idsCorretos.Count == lampioesAcesosAtualmente.Count && 
                             idsCorretos.All(lampioesAcesosAtualmente.Contains);

        if (estadoCorreto)
        {
            Debug.Log("ESTADO CORRETO! Puzzle resolvido!");
            ResolverPuzzle();
        }
        else
        {
            Debug.Log("Estado ainda incorreto. Lampiões acesos: " + string.Join(", ", lampioesAcesosAtualmente));
        }
    }

    private void ResolverPuzzle()
    {
        puzzleResolvido = true;
        
        if (porta != null)
        {
            porta.Destrancar();
        }
        
        // Opcional: Desabilitar a interação com os botões após resolver.
        foreach (LampiaoController lampiao in lampioes)
        {
            lampiao.enabled = false;
        }
    }

    // --- Precisamos adicionar duas pequenas funções no LampiaoController ---
}