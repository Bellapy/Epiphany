using UnityEngine;

public class AudioEventBridge : MonoBehaviour
{
    public void StopMusicWithDefaultFade()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusicWithFade(1.5f); 
        }
    } 

    public void PlayMusic(AudioClip musicClip)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(musicClip);
        }
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxClip);
        }
    }
}