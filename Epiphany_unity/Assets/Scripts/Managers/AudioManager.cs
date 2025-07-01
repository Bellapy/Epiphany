// Em _Scripts/Managers/AudioManager.cs
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fontes de Áudio")]
    [SerializeField] private AudioSource musicSource; // Toca as músicas de fundo
    [SerializeField] private AudioSource sfxSource;   // Toca os efeitos sonoros

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- MÉTODOS PÚBLICOS PARA CONTROLAR O ÁUDIO ---

    /// <summary>
    /// Toca uma música de fundo. Para a música atual antes de tocar a nova.
    /// </summary>
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;
        
        // Para evitar tocar a mesma música de novo se ela já estiver tocando
        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        musicSource.clip = musicClip;
        musicSource.Play();
    }

    /// <summary>
    /// Toca uma música de fundo com um fade in suave.
    /// </summary>
    public void PlayMusicWithFade(AudioClip musicClip, float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeInMusic(musicClip, fadeDuration));
    }

    /// <summary>
    /// Para a música atual com um fade out suave.
    /// </summary>
    public void StopMusicWithFade(float fadeDuration = 1.0f)
    {
        StartCoroutine(FadeOutMusic(fadeDuration));
    }

    /// <summary>
    /// Toca um efeito sonoro uma única vez.
    /// </summary>
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;
        sfxSource.PlayOneShot(sfxClip);
    }

    // --- CORROTINAS PARA OS FADES ---

    private IEnumerator FadeInMusic(AudioClip musicClip, float duration)
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }

        float startVolume = 0;
        musicSource.volume = startVolume;

        while (musicSource.volume < 1.0f)
        {
            musicSource.volume += Time.deltaTime / duration;
            yield return null;
        }
        musicSource.volume = 1.0f;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // Reseta o volume para o próximo fade in
    }
}
