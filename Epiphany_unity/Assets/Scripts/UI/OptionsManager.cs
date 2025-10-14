using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    [Header("Referências")]
    [Tooltip("Arraste o PREFAB do seu painel de opções para cá.")]
    [SerializeField] private GameObject optionsPanelPrefab;

    [Header("Canvas Persistente")]
    [Tooltip("Arraste o Canvas que deve sobreviver entre as cenas aqui.")]
    [SerializeField] private Canvas persistentCanvas;
    
    private GameObject optionsPanelInstance;
    private Slider volumeSlider;

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string masterVolumeParameterName = "MasterVolume";

    private const string VOLUME_PREF_KEY = "MasterVolume";
    private bool isOptionsOpen = false;

    void Awake()
    {
        // --- LOG DE DEPURAÇÃO 1 ---
        Debug.Log($"[OptionsManager] Awake() chamado no GameObject '{gameObject.name}'.");

        if (Instance != null && Instance != this)
        {
            // --- LOG DE DEPURAÇÃO 2 ---
            Debug.LogWarning($"[OptionsManager] Instância duplicada detectada! Destruindo '{gameObject.name}'. O Singleton original está em '{Instance.gameObject.name}'.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // --- LOG DE DEPURAÇÃO 3 ---
        Debug.Log($"[OptionsManager] '{gameObject.name}' foi definido como o Singleton. Tornando persistente.");

        if (persistentCanvas != null)
        {
            DontDestroyOnLoad(persistentCanvas.gameObject);
        }
        else
        {
            Debug.LogError("[OptionsManager] ERRO CRÍTICO: O Persistent Canvas não foi atribuído no Inspector!");
            return;
        }

        if (optionsPanelPrefab != null)
        {
            optionsPanelInstance = Instantiate(optionsPanelPrefab);
            optionsPanelInstance.transform.SetParent(persistentCanvas.transform, false);
            optionsPanelInstance.SetActive(false);

            volumeSlider = optionsPanelInstance.GetComponentInChildren<Slider>();
            Button backButton = null;
            Button[] buttons = optionsPanelInstance.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == "botaoVoltar")
                {
                    backButton = button;
                    break;
                }
            }

            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }
            if (backButton != null)
            {
                backButton.onClick.AddListener(CloseOptions);
            }
        }

        if (volumeSlider != null && masterMixer != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 0f);
            volumeSlider.value = savedVolume;
            SetMasterVolume(savedVolume);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // --- LOG DE DEPURAÇÃO 4 ---
            Debug.Log($"[OptionsManager] Tecla 'F' pressionada! Chamando ToggleOptions().");
            ToggleOptions();
        }
    }

    public void ToggleOptions()
    {
        isOptionsOpen = !isOptionsOpen;
        if (isOptionsOpen) { OpenOptions(); }
        else { CloseOptions(); }
    }

    public void OpenOptions()
    {
        isOptionsOpen = true;
        if (optionsPanelInstance != null)
        {
            optionsPanelInstance.SetActive(true);
        }
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseOptions()
    {
        isOptionsOpen = false;
        if (optionsPanelInstance != null)
        {
            optionsPanelInstance.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void SetMasterVolume(float volume)
    {
        if (masterMixer != null)
        {
            masterMixer.SetFloat(masterVolumeParameterName, volume);
            PlayerPrefs.SetFloat(VOLUME_PREF_KEY, volume);
            PlayerPrefs.Save();
        }
    }
}