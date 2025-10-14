using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour 
{
    public string sceneToLoad = "Scene2"; 
    public string spawnPointNameInNextScene = "EntryFromScene1"; 



    void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player tocou na porta. Iniciando transição automática.");

           
            StartSceneTransition();
        }
    }

    void StartSceneTransition()
    {
  
        if (GameManager.Instance != null)
        {
            
            GameManager.Instance.SetNextSpawnPoint(spawnPointNameInNextScene);
            Debug.Log($"[DoorTrigger] Solicitando ao GameManager para definir próximo spawn: {spawnPointNameInNextScene}");

       
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("[DoorTrigger] Instância do GameManager não encontrada! Não é possível definir o ponto de spawn ou trocar de cena.");
        }
    }

    
}