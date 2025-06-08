using UnityEngine;
using UnityEngine.UI; // Para o Slider
using UnityEngine.Audio; // Para o AudioMixer

public class OptionsManager : MonoBehaviour
{
    [Header("Referências UI")]
    [Tooltip("O GameObject do painel de opções completo.")]
    public GameObject optionsPanel;

    [Tooltip("O Slider de volume no painel de opções.")]
    public Slider volumeSlider;

    [Header("Configurações de Áudio")]
    [Tooltip("O Audio Mixer principal que controla o volume geral.")]
    public AudioMixer masterMixer;

    [Tooltip("O nome do parâmetro de volume exposto no Audio Mixer (ex: MasterVolume).")]
    public string masterVolumeParameterName = "MasterVolume"; // Certifique-se que este nome é o mesmo que você expôs no mixer!

    private const string VOLUME_PREF_KEY = "MasterVolume"; // Chave para salvar/carregar o volume

    void Awake()
    {
        // Garante que o painel de opções comece inativo ao carregar a cena
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        // Carrega o volume salvo e define o slider
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 0f); // 0f é o valor padrão se não houver volume salvo
            volumeSlider.value = savedVolume;
            SetMasterVolume(savedVolume); // Aplica o volume salvo ao mixer
        }
    }

    // Método para abrir o painel de opções
    public void OpenOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            // Opcional: Pausar o jogo quando o menu de opções abrir
            // Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("OptionsPanel não atribuído no OptionsManager.");
        }
    }

    // Método para fechar o painel de opções
    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            // Opcional: Retomar o jogo quando o menu de opções fechar
            // Time.timeScale = 1f;
        }
        else
        {
            Debug.LogWarning("OptionsPanel não atribuído no OptionsManager.");
        }
    }

    // Método chamado pelo Slider para mudar o volume
    public void SetMasterVolume(float volume)
    {
        if (masterMixer != null)
        {
            // Para AudioMixers, o volume é definido em decibéis.
            // Mathf.Log10(volume) * 20 transforma um valor linear (0-1) em dB.
            // Mas como nosso slider vai de -80 a 0, basta usar o valor direto.
            masterMixer.SetFloat(masterVolumeParameterName, volume);
            PlayerPrefs.SetFloat(VOLUME_PREF_KEY, volume); // Salva o volume
            PlayerPrefs.Save(); // Salva as PlayerPrefs imediatamente
        }
        else
        {
            Debug.LogWarning("AudioMixer não atribuído no OptionsManager.");
        }
    }
}