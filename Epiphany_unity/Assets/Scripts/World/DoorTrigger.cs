using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour 
{
    [Header("Configuração da Transição")]
    public string sceneToLoad = "Scene2"; 
    public string spawnPointNameInNextScene = "EntryFromScene1"; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartSceneTransition();
        }
    }

    void StartSceneTransition()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetNextSpawnPoint(spawnPointNameInNextScene);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}