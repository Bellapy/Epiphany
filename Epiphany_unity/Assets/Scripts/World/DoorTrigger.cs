using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour // Ou TrocaDeCena, se preferir esse nome
{
    public string sceneToLoad = "Scene2"; // Nome da cena para onde a porta leva
    public string spawnPointNameInNextScene = "EntryFromScene1"; // Nome do GameObject de spawn na PRÓXIMA cena

    // Não precisamos mais da variável 'playerIsNear' nem dos métodos Update() e OnTriggerExit2D()
    // para a transição automática.

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Verifica se o objeto que entrou no trigger tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player tocou na porta. Iniciando transição automática.");

            // 2. Chama o método para iniciar a transição de cena
            StartSceneTransition();
        }
    }

    void StartSceneTransition()
    {
        // 3. Verifica se a instância do GameManager existe
        if (GameManager.Instance != null)
        {
            // 4. Informa ao GameManager qual o ponto de spawn na próxima cena
            GameManager.Instance.SetNextSpawnPoint(spawnPointNameInNextScene);
            Debug.Log($"[DoorTrigger] Solicitando ao GameManager para definir próximo spawn: {spawnPointNameInNextScene}");

            // Opcional: Adicionar um fade out aqui antes de carregar a cena.
            // Se você tiver um sistema de fade como o do CutsceneTransitionManager,
            // você pode chamá-lo aqui e esperar ele terminar antes de carregar a cena.
            // Para isso, StartSceneTransition precisaria ser uma Coroutine e você chamaria
            // StartCoroutine(StartSceneTransition()) no OnTriggerEnter2D.
            // Exemplo:
            // StartCoroutine(HandleFadeAndLoad());

            // 5. Carrega a nova cena
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("[DoorTrigger] Instância do GameManager não encontrada! Não é possível definir o ponto de spawn ou trocar de cena.");
        }
    }

    
}