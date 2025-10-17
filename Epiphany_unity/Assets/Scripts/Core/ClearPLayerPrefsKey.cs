using UnityEngine;
using UnityEditor; // Precisamos disso para criar um item de menu

public class ClearPlayerPrefsKey
{
    // Define o nome da chave que queremos limpar.
    private const string keyToClear = "AylaForestTourCompleted";

    [MenuItem("Ferramentas/Limpar Flag do Tour da Floresta")]
    private static void ClearTourFlag()
    {
        if (PlayerPrefs.HasKey(keyToClear))
        {
            PlayerPrefs.DeleteKey(keyToClear);
            PlayerPrefs.Save();
            Debug.Log($"[DEBUG] A chave PlayerPrefs '{keyToClear}' foi limpa com sucesso!");
        }
        else
        {
            Debug.LogWarning($"[DEBUG] A chave PlayerPrefs '{keyToClear}' não foi encontrada. Nenhuma ação foi necessária.");
        }
    }
}