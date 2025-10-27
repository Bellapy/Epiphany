using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // --- INÍCIO DA LÓGICA DO SINGLETON ---
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        // Se NÃO existe nenhuma instância ainda...
        if (Instance == null)
        {
            // ...eu me torno a instância.
            Instance = this;
            // E eu não devo ser destruído ao carregar novas cenas.
            DontDestroyOnLoad(gameObject);
        }
        // Se uma instância JÁ EXISTE e não sou eu...
        else if (Instance != this)
        {
            // ...então eu sou uma duplicata desnecessária. Me destruo.
            Destroy(gameObject);
            // Retornar aqui é crucial para não executar o resto do Awake()
            return;
        }
    }
    // --- FIM DA LÓGICA DO SINGLETON ---

    [Header("Fontes de Áudio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private List<GameObject> activeSfxObjects = new List<GameObject>();

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || musicSource == null) return;
        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlayMusicWithFade(AudioClip musicClip, float fadeDuration = 1.0f)
    {
        if (musicSource == null) return;
        // IMPORTANTE: A corrotina agora é iniciada na instância Singleton (Instance)
        // para garantir que funcione mesmo se uma duplicata tentar chamar.
        Instance.StartCoroutine(FadeInMusic(musicClip, fadeDuration));
    }

    public void StopMusicWithFade(float fadeDuration = 1.0f)
    {
        if (musicSource == null) return;
        Instance.StartCoroutine(FadeOutMusic(fadeDuration));
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        PlaySFXAtPoint(sfxClip, Vector3.zero);
    }

    public void PlaySFXAtPoint(AudioClip sfxClip, Vector3 position)
    {
        if (sfxClip == null) return;

        GameObject tempAudioObject = new GameObject("TempSFX_" + sfxClip.name);
        activeSfxObjects.Add(tempAudioObject);
        tempAudioObject.transform.position = position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        
        audioSource.clip = sfxClip;
        audioSource.spatialBlend = 0.0f;
        audioSource.Play();

        Instance.StartCoroutine(DestroyAfterPlaying(tempAudioObject, sfxClip.length));
    }

    public void StopAllSFX()
    {
        foreach (GameObject sfxObject in new List<GameObject>(activeSfxObjects))
        {
            if (sfxObject != null)
            {
                Destroy(sfxObject);
            }
        }
        activeSfxObjects.Clear();
    }

    private IEnumerator DestroyAfterPlaying(GameObject objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeSfxObjects.Remove(objectToDestroy);
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }
    }

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
        musicSource.volume = startVolume;
    }
}