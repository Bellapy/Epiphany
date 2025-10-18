using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationLoader : MonoBehaviour
{
   
    void Awake()
    {
        
        if (GameManager.Instance == null)
        {
            Debug.Log("Managers não encontrados. Carregando a cena Initializer...");
           
            SceneManager.LoadScene("Initializer", LoadSceneMode.Additive);
        }
    }
}