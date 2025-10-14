using UnityEngine;

public class AudioEventBridge : MonoBehaviour
{
    public void StopMusicWithDefaultFade()
    {
        if (AudioManager.Instance != null)
        {
            Debug.Log("[AudioEventBridge] Chamando StopMusicWithFade no AudioManager.Instance.");
            AudioManager.Instance.StopMusicWithFade(1.5f); 
        }
        else
        {
            Debug.LogWarning("[AudioEventBridge] Tentou parar a música, mas o AudioManager.Instance não foi encontrado!");
        }
    } 

    public void PlayMusic(AudioClip musicClip)
    {
        if (AudioManager.Instance != null)
        {
            Debug.Log($"[AudioEventBridge] Chamando PlayMusic no AudioManager.Instance com o clipe '{musicClip.name}'.");
            AudioManager.Instance.PlayMusic(musicClip);
        }
        else
        {
            Debug.LogWarning("[AudioEventBridge] Tentou tocar música, mas o AudioManager.Instance não foi encontrado!");
        }
    }
    // <<< A CHAVE EXTRA QUE ESTAVA AQUI FOI REMOVIDA >>>

    public void PlaySFX(AudioClip sfxClip)
    {
        if (AudioManager.Instance != null)
        {
            Debug.Log($"[AudioEventBridge] Chamando PlaySFX no AudioManager.Instance com o clipe '{sfxClip.name}'.");
            AudioManager.Instance.PlaySFX(sfxClip);
        }
        else
        {
            Debug.LogWarning("[AudioEventBridge] Tentou tocar SFX, mas o AudioManager.Instance não foi encontrado!");
        }
    }
} // <<< A CHAVE FINAL AGORA FECHA A CLASSE CORRETAMENTE, INCLUINDO TODOS OS MÉTODOS