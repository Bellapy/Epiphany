using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider volumeSlider;
    private const string MIXER_VOLUME_PARAM = "MasterVolume";
    private const string VOLUME_PREF_KEY = "MasterVolumePreference";

    private bool isPaused = false;

    void Start()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);

        if (volumeSlider != null && masterMixer != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 1f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
            
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        optionsPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log($"[PauseMenuManager] TogglePause chamado. Jogo pausado: {isPaused}");
    }

    public void OnVolumeSliderChanged(float value)
    {
        SetVolume(value);
    }

    private void SetVolume(float linearValue)
    {
        // Garante que o valor linear nunca seja zero para evitar -Infinity dB
        if (linearValue <= 0)
        {
            linearValue = 0.0001f;
        }
        
        float dbValue = Mathf.Log10(linearValue) * 20;
        masterMixer.SetFloat(MIXER_VOLUME_PARAM, dbValue);
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, linearValue);
    }
}