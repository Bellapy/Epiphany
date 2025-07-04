// Em _Scripts/World/SceneMusicTrigger.cs
using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("Configuração da Música")]
    [Tooltip("A música que deve começar a tocar nesta cena.")]
    public AudioClip sceneMusic;
    
    [Tooltip("Duração do fade in da música ao entrar na cena.")]
    public float fadeInDuration = 1.0f;

    void Start()
    {
        if (sceneMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusicWithFade(sceneMusic, fadeInDuration);
        }
    }
}
