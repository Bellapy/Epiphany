using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuPrincipalManager : MonoBehaviour
{
    // Propriedade estática para implementar o padrão Singleton.
    // Permite que outros scripts acessem este manager globalmente via "MenuPrincipalManager.Instance".

    [Header("Configuração de Cena")]
    [Tooltip("O nome exato da cena principal do jogo a ser carregada.")]
    [SerializeField] private string nomeDoLevelDeJogo;


    /// <summary>
    /// Método público para ser chamado pelo botão "Jogar".
    /// Inicia a transição de cena com um fade out.
    /// </summary>
    public void Jogar()
    {
        // Verifica se o FadeController está disponível para uma transição suave.
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeOut(CarregarCenaPrincipal);
        }
        else
        {
            // Se não houver FadeController, carrega a cena diretamente como fallback.
            Debug.LogWarning("FadeController.Instance não encontrado. Carregando cena diretamente.");
            CarregarCenaPrincipal();
        }
    }

    /// <summary>
    /// Método privado que efetivamente carrega a cena do jogo.
    /// É chamado como um callback pelo FadeController após o fade out terminar.
    /// </summary>
    private void CarregarCenaPrincipal()
    {
        if (!string.IsNullOrEmpty(nomeDoLevelDeJogo))
        {
            SceneManager.LoadScene(nomeDoLevelDeJogo);
        }
        else
        {
            Debug.LogError("O nome da cena do level de jogo não foi definido no MenuPrincipalManager!");
        }
    }
}