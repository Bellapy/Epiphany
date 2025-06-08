// MenuPrincipalManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necessário para IEnumerator se você quiser fazer o fade diretamente aqui

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    // [SerializeField] private FadeController fadeController; // Opção 1: Arrastar referência
                                                            // Opção 2 (usando Singleton): Não precisa arrastar

    public void Jogar()
    {
        // Opção 1: Se você arrastou a referência do FadeController no Inspector:
        // if (fadeController != null)
        // {
        //     fadeController.StartFadeOut(CarregarCenaPrincipal);
        // }
        // else
        // {
        //     Debug.LogWarning("FadeController não atribuído no MenuPrincipalManager. Carregando cena diretamente.");
        //     CarregarCenaPrincipal(); // Carrega a cena sem fade se o controller não estiver lá
        // }

        // Opção 2: Usando o Singleton do FadeController (mais simples de configurar)
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeOut(CarregarCenaPrincipal);
        }
        else
        {
            Debug.LogWarning("FadeController.Instance não encontrado. Carregando cena diretamente.");
            CarregarCenaPrincipal();
        }
    }

    private void CarregarCenaPrincipal()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    // Se você quiser adicionar um botão de Sair com fade também:
    // public void SairDoJogo()
    // {
    //     if (FadeController.Instance != null)
    //     {
    //         FadeController.Instance.StartFadeOut(() => {
    //             Debug.Log("Saindo do jogo...");
    //             Application.Quit();
    //             #if UNITY_EDITOR
    //             UnityEditor.EditorApplication.isPlaying = false; // Para parar no editor
    //             #endif
    //         });
    //     }
    //     else
    //     {
    //         Debug.Log("Saindo do jogo...");
    //         Application.Quit();
    //         #if UNITY_EDITOR
    //         UnityEditor.EditorApplication.isPlaying = false;
    //         #endif
    //     }
    // }
}