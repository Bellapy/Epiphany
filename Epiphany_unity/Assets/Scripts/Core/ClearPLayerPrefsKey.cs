using UnityEngine;
using UnityEditor;

public class ClearPlayerPrefsKey
{
    // Define os nomes das chaves que queremos poder limpar.
    private const string forestTourKey = "AylaForestTourCompleted";
    private const string stargazingKey = "StargazingSceneCompleted";

    // --- OPÇÃO 1: Limpar Flag do Tour da Floresta ---
    [MenuItem("Ferramentas/Limpar Flags/Limpar Flag do Tour da Floresta")]
    private static void ClearForestTourFlag()
    {
        if (PlayerPrefs.HasKey(forestTourKey))
        {
            PlayerPrefs.DeleteKey(forestTourKey);
            PlayerPrefs.Save();
            Debug.Log($"[DEBUG] A chave PlayerPrefs '{forestTourKey}' foi limpa com sucesso!");
        }
        else
        {
            Debug.LogWarning($"[DEBUG] A chave PlayerPrefs '{forestTourKey}' não foi encontrada. Nenhuma ação foi necessária.");
        }
    }

    // --- OPÇÃO 2: Limpar Flag da Cena de Observação (NOVA) ---
    [MenuItem("Ferramentas/Limpar Flags/Limpar Flag da Cena de Observação")]
    private static void ClearStargazingFlag()
    {
        if (PlayerPrefs.HasKey(stargazingKey))
        {
            PlayerPrefs.DeleteKey(stargazingKey);
            PlayerPrefs.Save();
            Debug.Log($"[DEBUG] A chave PlayerPrefs '{stargazingKey}' foi limpa com sucesso!");
        }
        else
        {
            Debug.LogWarning($"[DEBUG] A chave PlayerPrefs '{stargazingKey}' não foi encontrada. Nenhuma ação foi necessária.");
        }
    }

    // --- OPÇÃO 3: Limpar TODAS as Flags do Jogo (BÔNUS) ---
    [MenuItem("Ferramentas/Limpar Flags/Limpar TODAS as Flags do Jogo")]
    private static void ClearAllGameFlags()
    {
        // Esta função limpa todas as chaves que definimos.
        // É útil para começar um teste "do zero absoluto".
        PlayerPrefs.DeleteKey(forestTourKey);
        PlayerPrefs.DeleteKey(stargazingKey);
        // Adicione outras chaves aqui no futuro, se necessário.
        
        PlayerPrefs.Save();
        Debug.Log($"[DEBUG] TODAS as flags de progresso do jogo foram limpas!");
    }
}