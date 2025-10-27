using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    

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