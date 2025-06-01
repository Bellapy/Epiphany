using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string sceneToLoad;       // nome da cena que será carregada
    public string spawnPointName;    // nome do GameObject que será o ponto de spawn

    public void OnPlayerEnterDoor()
    {
        // Define o próximo ponto de spawn para o player
        NewMonoBehaviourScript.SetNextSpawnPoint(spawnPointName);

        // Carrega a nova cena
        SceneManager.LoadScene(sceneToLoad);
    }
}
