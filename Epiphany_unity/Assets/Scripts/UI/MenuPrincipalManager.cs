// MenuPrincipalManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;


    public void Jogar()
    {
        
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

   
}