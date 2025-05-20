using UnityEngine;
using UnityEngine.SceneManagement; // Namespace correto para SceneManager

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo; // Corrigido: SerializeField e nome da variável

    public void Jogar()
    {
       
            SceneManager.LoadScene(nomeDoLevelDeJogo); // Usa a variável e a classe correta

       
    }

}
