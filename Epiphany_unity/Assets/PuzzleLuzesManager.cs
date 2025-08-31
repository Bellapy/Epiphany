using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PuzzleLuzesManager : MonoBehaviour
{
    [Header("Configuração do Puzzle")]
    [Tooltip("Defina os IDs dos lampiões que devem estar ACESOS para resolver o puzzle.")]
    [SerializeField] private List<int> idsCorretos = new List<int>();

    [Header("Referências da Cena")]
    [Tooltip("Arraste todos os scripts LampiaoController para esta lista.")]
    [SerializeField] private List<LampiaoController> lampioes;

    /// <summary>
    /// Verifica se o estado atual dos lampiões corresponde à solução.
    /// </summary>
    public bool VerificarSolucao()
    {
        List<int> lampioesAcesosAtualmente = new List<int>();
        foreach (LampiaoController lampiao in lampioes)
        {
            if (lampiao != null && lampiao.EstaAceso())
            {
                lampioesAcesosAtualmente.Add(lampiao.GetID());
            }
        }

        bool estadoCorreto = idsCorretos.Count == lampioesAcesosAtualmente.Count && 
                             idsCorretos.All(lampioesAcesosAtualmente.Contains);

        return estadoCorreto;
    }

    /// <summary>
    /// Manda todos os lampiões da lista se apagarem.
    /// </summary>
    public void ResetarTodosLampioes()
    {
        Debug.Log("Resetando o estado de todos os lampiões...");
        foreach (LampiaoController lampiao in lampioes)
        {
            if (lampiao != null)
            {
                lampiao.ResetarLampiao();
            }
        }
    }
}