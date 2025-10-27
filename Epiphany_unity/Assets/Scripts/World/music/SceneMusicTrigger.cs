using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("Configuração da Música")]
    public AudioClip sceneMusic;
    public float fadeInDuration = 1.0f;

    void Start()
    {
        // Acessa a instância global diretamente.
        if (AudioManager.Instance != null && sceneMusic != null)
        {
            AudioManager.Instance.PlayMusicWithFade(sceneMusic, fadeInDuration);
        }
        else
        {
            Debug.LogWarning("[SceneMusicTrigger] AudioManager.Instance não encontrado ou nenhuma música foi definida.");
        }
    }
}