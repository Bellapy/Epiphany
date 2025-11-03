using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // --- INÍCIO DA LÓGICA DO SINGLETON ---
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    // --- FIM DA LÓGICA DO SINGLETON ---

    public string NextPlayerSpawnPointName { get; private set; }
    public Vector3 NextPlayerPosition { get; private set; }
    private bool useSpecificPositionForSpawn = false;

    private List<string> objectsToActivateOnLoad = new List<string>();

    [Header("Flags de Estado da História")]
    public bool HasCompletedKitchenDialogue { get; set; } = false;
    public bool PlayerHasFlute { get; set; } = false;
    public bool HasCompletedKitchenSequence { get; set; } = false;

    // <<< NOVA FLAG ADICIONADA AQUI >>>
    public bool HasSolvedCorridorPuzzle { get; set; } = false;
    // <<< FIM DA ADIÇÃO >>>

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetNextSpawnPoint(string spawnPointName)
    {
        NextPlayerSpawnPointName = spawnPointName;
        useSpecificPositionForSpawn = false;
    }

    public void SetNextSpawnPosition(Vector3 position)
    {
        NextPlayerPosition = position;
        useSpecificPositionForSpawn = true;
        NextPlayerSpawnPointName = null;
    }

    public void AddObjectToActivateOnLoad(string objectName)
    {
        if (!objectsToActivateOnLoad.Contains(objectName))
        {
            objectsToActivateOnLoad.Add(objectName);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PositionPlayerInScene();
        ActivateScheduledObjects();
    }

    private void PositionPlayerInScene()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;
        
        if (useSpecificPositionForSpawn)
        {
            player.transform.position = NextPlayerPosition;
        }
        else if (!string.IsNullOrEmpty(NextPlayerSpawnPointName))
        {
            GameObject spawnPointObject = GameObject.Find(NextPlayerSpawnPointName);
            if (spawnPointObject != null)
            {
                player.transform.position = spawnPointObject.transform.position;
            }
            else
            {
                Debug.LogWarning($"[GameManager] Aviso: Ponto de spawn '{NextPlayerSpawnPointName}' não encontrado na cena '{SceneManager.GetActiveScene().name}'.");
            }
        }

        NextPlayerSpawnPointName = null;
        useSpecificPositionForSpawn = false;
    }

    private void ActivateScheduledObjects()
    {
        if (objectsToActivateOnLoad.Count == 0) return;

        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (string objectName in objectsToActivateOnLoad)
        {
            GameObject objectToActivate = allObjects.FirstOrDefault(g => g.name == objectName && g.scene.isLoaded);
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"[GameManager] Objeto '{objectName}' agendado para ativação não foi encontrado na cena.");
            }
        }
        objectsToActivateOnLoad.Clear();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}