using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [Header("Painéis da UI")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private CanvasGroup fadePanelCanvasGroup;

    [Header("Configuração de Cena")]
    [SerializeField] private string gameSceneName = "AnimacaoInicial";
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider volumeSlider;
    private const string MIXER_VOLUME_PARAM = "MasterVolume";
    private const string VOLUME_PREF_KEY = "MasterVolumePreference";

    void Start()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (fadePanelCanvasGroup != null) fadePanelCanvasGroup.alpha = 0;

        if (volumeSlider != null && masterMixer != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 1f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
    }

    // --- INÍCIO DA ADIÇÃO ---
    void Update()
    {
        // Verifica se a tecla F foi pressionada
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Chama a mesma função que o botão de Opções chamaria, mas para alternar
            ToggleOptionsPanel();
        }
    }

    // Nova função para alternar o painel
    public void ToggleOptionsPanel()
    {
        if (optionsPanel != null)
        {
            bool isActive = optionsPanel.activeSelf;
            optionsPanel.SetActive(!isActive);
            Debug.Log($"[MainMenuController] Painel de Opções alternado. Ativo: {!isActive}");
        }
    }
    // --- FIM DA ADIÇÃO ---

    // --- Funções para os botões ---

    public void OnPlayButtonClicked()
    {
        Debug.Log("Botão JOGAR clicado. Iniciando fade-out...");
        StartCoroutine(FadeOutAndLoadScene());
    }

    public void OnOptionsButtonClicked()
    {
        Debug.Log("Botão OPÇÕES clicado.");
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Botão SAIR clicado.");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnBackButtonClicked()
    {
        Debug.Log("Botão VOLTAR clicado.");
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OnVolumeSliderChanged(float value)
    {
        SetVolume(value);
    }

    private void SetVolume(float linearValue)
    {
        float dbValue = Mathf.Log10(linearValue) * 20;
        masterMixer.SetFloat(MIXER_VOLUME_PARAM, dbValue);
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, linearValue);
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        if (fadePanelCanvasGroup == null)
        {
            SceneManager.LoadScene(gameSceneName);
            yield break;
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanelCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        fadePanelCanvasGroup.alpha = 1;

        SceneManager.LoadScene(gameSceneName);
    }
}