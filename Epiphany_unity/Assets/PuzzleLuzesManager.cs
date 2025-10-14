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

    public bool VerificarSolucao()
    {
      
        List<int> idsAcesos = lampioes
            .Where(l => l != null && l.EstaAceso())
            .Select(l => l.GetID())
            .ToList();

  
        if (idsAcesos.Count != idsCorretos.Count)
        {
            return false;
        }

     
        HashSet<int> setIdsAcesos = new HashSet<int>(idsAcesos);
        HashSet<int> setIdsCorretos = new HashSet<int>(idsCorretos);

   
        return setIdsAcesos.SetEquals(setIdsCorretos);
    }

    public void ResetarTodosLampioes()
    {
        foreach (LampiaoController lampiao in lampioes)
        {
            if (lampiao != null)
            {
                lampiao.ResetarLampiao();
            }
        }
    }
}