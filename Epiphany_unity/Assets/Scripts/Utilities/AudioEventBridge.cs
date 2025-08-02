using UnityEngine;

// Este script atua como uma ponte simples entre UnityEvents na cena
// e o Singleton do AudioManager.
public class AudioEventBridge : MonoBehaviour
{
    // --- Funções para MÚSICA DE FUNDO ---

    /// <summary>
    /// Para a música de fundo atual com um fade suave.
    /// A duração do fade é definida aqui para simplificar o UnityEvent.
    /// </summary>
    public void StopMusicWithDefaultFade()
    {
        // Verifica se o AudioManager existe antes de tentar usá-lo.
        if (AudioManager.Instance != null)
        {
            Debug.Log("[AudioEventBridge] Chamando StopMusicWithFade no AudioManager.Instance.");
            // Você pode ajustar a duração do fade padrão aqui. 1.5 segundos é um bom valor.
            AudioManager.Instance.StopMusicWithFade(1.5f); 
        }
        else
        {
            Debug.LogWarning("[AudioEventBridge] Tentou parar a música, mas o AudioManager.Instance não foi encontrado!");
        }
    }

    /// <summary>
    /// Toca uma nova música de fundo. O AudioClip é definido no Inspector.
    /// </summary>
    /// <param name="musicClip">O clipe de música a ser tocado.</param>
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


    // --- Funções para EFEITOS SONOROS (SFX) ---

    /// <summary>
    /// Toca um efeito sonoro. O AudioClip é definido no Inspector.
    /// </summary>
    /// <param name="sfxClip">O efeito sonoro a ser tocado.</param>
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
}
