// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para Scene, LoadSceneMode, SceneManager

public class GameManager : MonoBehaviour
{
    // Propriedade estática para acessar a única instância do GameManager
    public static GameManager Instance { get; private set; }

    // Informações para o spawn do jogador na próxima cena
    public string NextPlayerSpawnPointName { get; private set; }
    public Vector3 NextPlayerPosition { get; private set; } // Alternativa, se preferir passar a posição exata
    private bool useSpecificPositionForSpawn = false;

    // Você pode adicionar outras variáveis globais do jogo aqui
    // Ex: public int playerScore;
    // Ex: public bool isGamePaused;

    void Awake()
    {
        // Lógica do Singleton
        if (Instance == null)
        {
            // Se não há instância, esta se torna a instância
            Instance = this;

            // Não destrói este GameObject ao carregar uma nova cena
            DontDestroyOnLoad(gameObject);

            // Inscreve-se no evento sceneLoaded para executar lógica após o carregamento de uma cena
            SceneManager.sceneLoaded += OnSceneLoaded; // Agora o método OnSceneLoaded existe abaixo
        }
        else if (Instance != this) // Se já existe uma instância e não é esta
        {
            // Destrói este GameObject para garantir que haja apenas uma instância
            Destroy(gameObject);
            return; // Impede que o resto do Awake() seja executado para esta instância duplicada
        }
    }

    void OnDestroy()
    {
        // É uma boa prática remover a inscrição de eventos quando o objeto é destruído
        if (Instance == this) // Só remova se este for o singleton real
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Agora o método OnSceneLoaded existe abaixo
        }
    }

    // --- MÉTODO PARA O DoorTrigger/TrocaDeCena CHAMAR ---
    public void SetNextSpawnPoint(string spawnPointName)
    {
        NextPlayerSpawnPointName = spawnPointName;
        useSpecificPositionForSpawn = false; // Indica que usaremos o nome do objeto de spawn
        Debug.Log($"[GameManager] Próximo ponto de spawn definido por nome: {NextPlayerSpawnPointName}");
    }

    // --- MÉTODO ALTERNATIVO (OPCIONAL) PARA DEFINIR SPAWN POR POSIÇÃO EXATA ---
    public void SetNextSpawnPosition(Vector3 position)
    {
        NextPlayerPosition = position;
        useSpecificPositionForSpawn = true; // Indica que usaremos a posição exata
        NextPlayerSpawnPointName = null; // Limpa o nome, pois usaremos a posição
        Debug.Log($"[GameManager] Próxima posição de spawn definida: {NextPlayerPosition}");
    }

    // --- MÉTODO CHAMADO AUTOMATICAMENTE QUANDO UMA NOVA CENA É CARREGADA ---
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Cena '{scene.name}' (modo: {mode}) carregada.");
        PositionPlayerInScene();
    }

    // --- LÓGICA PARA POSICIONAR O JOGADOR NA CENA ---
    private void PositionPlayerInScene()
    {
        // Encontra o script do jogador na cena atual
        // Substitua NewMonoBehaviourScript pelo nome real do seu script de controle do jogador
        NewMonoBehaviourScript player = FindFirstObjectByType<NewMonoBehaviourScript>();

        if (player == null)
        {
            Debug.LogError("[GameManager] Jogador (NewMonoBehaviourScript) não encontrado na cena após o carregamento!");
            // Se você tivesse um prefab do jogador para instanciar, a lógica iria aqui.
            return;
        }

        // Decide como posicionar o jogador com base no que foi definido
        if (useSpecificPositionForSpawn)
        {
            player.transform.position = NextPlayerPosition;
            Debug.Log($"[GameManager] Jogador posicionado em (coordenada exata): {NextPlayerPosition}");
        }
        else if (!string.IsNullOrEmpty(NextPlayerSpawnPointName))
        {
            GameObject spawnPointObject = GameObject.Find(NextPlayerSpawnPointName);
            if (spawnPointObject != null)
            {
                player.transform.position = spawnPointObject.transform.position;
                Debug.Log($"[GameManager] Jogador posicionado no objeto de spawn: {spawnPointObject.name} ({spawnPointObject.transform.position})");
            }
            else
            {
                Debug.LogWarning($"[GameManager] Ponto de spawn '{NextPlayerSpawnPointName}' não encontrado na cena '{SceneManager.GetActiveScene().name}'. Jogador não foi movido.");
            }
        }
        else
        {
            Debug.Log("[GameManager] Nenhum ponto de spawn específico (nome ou posição) definido para o jogador nesta cena. Jogador permanecerá na posição padrão.");
        }

        // Limpa as informações de spawn para a próxima transição
        NextPlayerSpawnPointName = null;
        useSpecificPositionForSpawn = false;
    }
}