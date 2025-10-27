using UnityEngine;

public class StopMusicTrigger : MonoBehaviour
{
    [Header("Configuração do Fade")]
    [Tooltip("Duração do fade out da música ao entrar na área.")]
    [SerializeField] private float fadeOutDuration = 2.0f;

    [Tooltip("Marque se este gatilho deve ser destruído após o primeiro uso.")]
    [SerializeField] private bool destroyOnTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopMusicWithFade(fadeOutDuration);
            }

            if (destroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}