using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTransition : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public string sceneToLoad;
    public string playerTag = "Player";
    public string destinationSpawnPointName; // Nome do GameObject do ponto de spawn na CENA DE DESTINO

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"Personagem ({playerTag}) entrou no trigger da porta '{this.name}'."); // Mensagem para saber qual porta foi ativada
            Debug.Log($"Salvando ponto de spawn: '{destinationSpawnPointName}' para a cena '{sceneToLoad}'."); // <-- ADICIONADO PARA DEPURAR

            PlayerPrefs.SetString("LastEnteredDoor", destinationSpawnPointName);
            PlayerPrefs.Save(); 

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}