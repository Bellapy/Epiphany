using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    // Singleton: Uma forma de garantir que teremos apenas UM PauseMenuManager no jogo
    // e que ele seja facilmente acessível de qualquer outro script.
    public static PauseMenuManager Instance { get; private set; }

    [Header("Configuração do Menu")]
    [Tooltip("Arraste o Prefab 'PauseMenuContainer' aqui.")]
    [SerializeField] private GameObject pauseMenuPrefab;

    private GameObject pauseMenuInstance; // A cópia do menu que realmente existe na cena.
    private bool isPaused = false;

    private void Awake()
    {
        // Lógica do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Garante que nosso manager não seja destruído ao trocar de cena.
        DontDestroyOnLoad(gameObject);

        // Cria a instância do menu a partir do Prefab, mas a deixa escondida.
        if (pauseMenuPrefab != null)
        {
            pauseMenuInstance = Instantiate(pauseMenuPrefab);
            pauseMenuInstance.SetActive(false);
        }
    }

    void Update()
    {
        // Ouve a tecla 'F' para abrir/fechar o menu.
        if (Input.GetKeyDown(KeyCode.F))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        isPaused = true;
        // Congela o tempo no jogo. Essencial para pausar de verdade!
        Time.timeScale = 0f; 
        pauseMenuInstance.SetActive(true);
    }

    // Este método precisa ser público para que o botão "Voltar" possa chamá-lo.
    public void Resume()
    {
        isPaused = false;
        // Retorna o tempo ao normal.
        Time.timeScale = 1f;
        pauseMenuInstance.SetActive(false);
    }
}