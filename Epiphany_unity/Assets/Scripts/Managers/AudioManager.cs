using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necessário para List

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fontes de Áudio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // Lista para rastrear todos os SFX temporários que criamos.
    private List<GameObject> activeSfxObjects = new List<GameObject>();

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
        StartCoroutine(FadeInMusic(musicClip, fadeDuration));
    }

    public void StopMusicWithFade(float fadeDuration = 1.0f)
    {
        if (musicSource == null) return;
        StartCoroutine(FadeOutMusic(fadeDuration));
    }

    // Função de SFX antiga, agora redirecionada para a nova.
    public void PlaySFX(AudioClip sfxClip)
    {
        PlaySFXAtPoint(sfxClip, Vector3.zero);
    }

    // Função de teste que cria um AudioSource temporário.
    public void PlaySFXAtPoint(AudioClip sfxClip, Vector3 position)
    {
        if (sfxClip == null) return;

        GameObject tempAudioObject = new GameObject("TempSFX_" + sfxClip.name);
        activeSfxObjects.Add(tempAudioObject); // Adiciona à lista de rastreamento
        tempAudioObject.transform.position = position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        
        audioSource.clip = sfxClip;
        audioSource.spatialBlend = 0.0f;
        audioSource.Play();

        // Passamos a lista para a corrotina para que ela possa se remover
        StartCoroutine(DestroyAfterPlaying(tempAudioObject, sfxClip.length));
    }

    /// <summary>
    /// Para e destrói todos os efeitos sonoros temporários que estão tocando.
    /// </summary>
    public void StopAllSFX()
    {
        Debug.Log("[AudioManager] Parando todos os SFX...");
        // Itera sobre uma cópia da lista para poder modificar a original
        foreach (GameObject sfxObject in new List<GameObject>(activeSfxObjects))
        {
            if (sfxObject != null)
            {
                Destroy(sfxObject);
            }
        }
        activeSfxObjects.Clear(); // Limpa a lista
    }

    // --- CORROTINAS INTERNAS ---

    private IEnumerator DestroyAfterPlaying(GameObject objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Remove da lista antes de destruir
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