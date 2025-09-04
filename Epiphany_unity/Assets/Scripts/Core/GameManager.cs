using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para gerenciar cenas

public class GameManager : MonoBehaviour
{
    // --- Padrão Singleton ---
    // A única instância do GameManager acessível de qualquer lugar do jogo.
    public static GameManager Instance { get; private set; }

    // --- Informações de Spawn do Jogador (Para Transições de Cena) ---
    // Nome do GameObject na próxima cena que servirá como ponto de spawn.
    public string NextPlayerSpawnPointName { get; private set; }
    // Posição exata para spawn do jogador na próxima cena (alternativa ao nome).
    public Vector3 NextPlayerPosition { get; private set; }
    // Flag para decidir se usaremos o nome do ponto de spawn ou a posição exata.
    private bool useSpecificPositionForSpawn = false;

    // --- Outras Variáveis Globais do Jogo (Exemplos) ---
    // Você pode adicionar variáveis como pontuação, estado do jogo (pausado/rodando), etc.
    // public int playerScore;
    // public bool isGamePaused;

    // --- Ciclo de Vida: Awake() ---
    // Chamado quando o script é carregado, antes de qualquer método Start().
    // Ideal para inicializar o Singleton.
    void Awake()
    {
        // Verifica se já existe uma instância do GameManager
        if (Instance == null)
        {
            // Se não existe nenhuma instância, esta se torna a única instância.
            Instance = this;
            Debug.Log("GameManager inicializado como Singleton.");

            // Garante que este GameObject não será destruído ao carregar novas cenas.
            // Isso é crucial para Managers que precisam persistir durante todo o jogo.
            DontDestroyOnLoad(gameObject);

            // Inscreve-se no evento 'sceneLoaded'. Isso faz com que o método OnSceneLoaded()
            // seja chamado automaticamente toda vez que uma nova cena for carregada.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            // Se já existe uma instância diferente desta, significa que há um duplicado.
            // Destrói este GameObject para garantir que apenas uma instância persista.
            Debug.LogWarning("GameManager duplicado detectado! Destruindo cópia para manter Singleton único.");
            Destroy(gameObject);
            return; // Sai do Awake() para evitar execução de código desnecessário.
        }
    }

    // --- Ciclo de Vida: OnDestroy() ---
    // Chamado quando o GameObject é destruído.
    // É importante remover a inscrição de eventos para evitar erros (memory leaks)
    // se o GameManager for destruído (ex: ao sair do jogo ou se for um duplicado).
    void OnDestroy()
    {
        // Só remove a inscrição se esta instância for a que estava ativa como Singleton.
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Remove a inscrição do evento.
            Debug.Log("GameManager destruído. Eventos desinscritos.");
        }
    }

    // --- Métodos de Controle de Cena e Spawn ---

    // Método para carregar uma nova cena.
    // Outros scripts (ex: MenuPrincipalManager, DoorTrigger) podem chamar isso.
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[GameManager] Solicitando carregamento da cena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Método para definir o ponto de spawn do jogador na próxima cena usando o NOME de um GameObject.
    // Ex: Chamado por um DoorTrigger ao mudar de cena.
    public void SetNextSpawnPoint(string spawnPointName)
    {
        NextPlayerSpawnPointName = spawnPointName;
        useSpecificPositionForSpawn = false; // Indica que usaremos o nome do objeto de spawn.
        Debug.Log($"[GameManager] Próximo ponto de spawn definido por nome: '{NextPlayerSpawnPointName}'.");
    }

    // Método alternativo para definir o spawn do jogador por uma POSIÇÃO EXATA.
    // Pode ser útil para teletransportes ou spawns dinâmicos.
    public void SetNextSpawnPosition(Vector3 position)
    {
        NextPlayerPosition = position;
        useSpecificPositionForSpawn = true; // Indica que usaremos a posição exata.
        NextPlayerSpawnPointName = null; // Limpa o nome, pois a posição tem prioridade.
        Debug.Log($"[GameManager] Próxima posição de spawn definida: {NextPlayerPosition}.");
    }

    // --- Evento de Carregamento de Cena ---
    // Este método é chamado automaticamente pelo Unity cada vez que uma cena é carregada.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Cena '{scene.name}' (modo: {mode}) carregada. Posicionando jogador...");
        PositionPlayerInScene(); // Chama a lógica para posicionar o jogador.
    }

    // --- Lógica Interna: Posicionar o Jogador na Cena ---
   private void PositionPlayerInScene()
{
    // Encontra a instância do script PlayerController na cena atual.
    PlayerController player = FindFirstObjectByType<PlayerController>();

    // <<< A CORREÇÃO ESTÁ AQUI >>>
    // Se não encontrarmos um jogador, isso é normal para cenas de menu ou finais.
    // Apenas registramos uma mensagem informativa e saímos da função.
    if (player == null)
    {
        Debug.Log("[GameManager] Nenhum jogador (PlayerController) encontrado na cena. Isso é normal para cenas de menu/finais. Nenhuma ação de posicionamento será tomada.");
        return; // Sai da função para evitar erros.
    }

    // O resto da sua lógica original só será executado se um jogador for encontrado.
    // Decide como posicionar o jogador com base nas informações de spawn definidas.
    if (useSpecificPositionForSpawn)
    {
        player.transform.position = NextPlayerPosition;
        Debug.Log($"[GameManager] Jogador posicionado em (coordenada exata): {NextPlayerPosition}.");
    }
    else if (!string.IsNullOrEmpty(NextPlayerSpawnPointName))
    {
        // Tenta encontrar um GameObject com o nome do ponto de spawn na cena.
        GameObject spawnPointObject = GameObject.Find(NextPlayerSpawnPointName);
        if (spawnPointObject != null)
        {
            player.transform.position = spawnPointObject.transform.position;
            Debug.Log($"[GameManager] Jogador posicionado no objeto de spawn: '{spawnPointObject.name}' ({spawnPointObject.transform.position}).");
        }
        else
        {
            Debug.LogWarning($"[GameManager] Aviso: Ponto de spawn '{NextPlayerSpawnPointName}' não encontrado na cena '{SceneManager.GetActiveScene().name}'. O jogador permanecerá na posição padrão da cena.");
        }
    }
    else
    {
        Debug.Log("[GameManager] Nenhum ponto de spawn específico (nome ou posição) foi definido. Jogador permanecerá na posição inicial da cena.");
    }

    // Limpa as informações de spawn para a próxima transição de cena.
    NextPlayerSpawnPointName = null;
    useSpecificPositionForSpawn = false;
}

    // --- Outras Funções do Jogo (Exemplos) ---
    // Você pode adicionar métodos para pausar o jogo, gerenciar o estado da UI, etc.
    public void PauseGame()
    {
        // isGamePaused = true;
        Time.timeScale = 0f; // Pausa o tempo do jogo
        Debug.Log("Jogo Pausado.");
    }

    public void ResumeGame()
    {
        // isGamePaused = false;
        Time.timeScale = 1f; // Retoma o tempo do jogo
        Debug.Log("Jogo Resumido.");
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        #if UNITY_EDITOR // Se estiver no editor da Unity
        UnityEditor.EditorApplication.isPlaying = false; // Para o modo de jogo no editor
        #else // Se for uma build
        Application.Quit(); // Fecha o aplicativo
        #endif
    }
}