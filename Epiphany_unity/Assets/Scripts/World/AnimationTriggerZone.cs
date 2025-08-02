using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AnimationTriggerZone : MonoBehaviour
{
    [Header("Configuração do Gatilho")]
    [Tooltip("Arraste aqui o Animator que você quer ativar.")]
    [SerializeField] private Animator targetAnimator;
    
    [Tooltip("O nome exato do Trigger no Animator Controller.")]
    [SerializeField] private string triggerName = "Activate";

    [Tooltip("Marque se este gatilho deve funcionar apenas uma vez.")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasBeenTriggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasBeenTriggered) return;

        if (other.CompareTag("Player"))
        {
            if (targetAnimator != null)
            {
                Debug.Log($"Ativando animação no '{targetAnimator.name}' com o gatilho '{triggerName}'.");
                
                // AQUI ESTÁ A MÁGICA: Diz ao Animator para disparar o gatilho.
                targetAnimator.SetTrigger(triggerName);

                hasBeenTriggered = true;
            }
            else
            {
                Debug.LogWarning("AnimationTriggerZone: Target Animator não foi configurado!");
            }
        }
    }
}
