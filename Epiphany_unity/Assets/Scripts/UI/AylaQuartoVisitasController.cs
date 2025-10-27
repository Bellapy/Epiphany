using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AylaQuartoVisitasController : MonoBehaviour
{
    [Header("Identificador de Estado")]
    [Tooltip("Nome único usado para salvar o progresso desta cena.")]
    [SerializeField] private string sceneCompletionFlag = "AylaQuartoVisitasCompleted";

    [Header("Configurações da Sequência")]
    [SerializeField] private float delayToStart = 3.0f;
    [SerializeField] private DialogueData sceneDialogue;
    [Tooltip("O índice da linha de diálogo que aciona a caminhada (começando em 0).")]
    [SerializeField] private int walkTriggerLineIndex = 4;
    [Tooltip("O índice da linha de diálogo que aciona o desaparecimento.")]
    [SerializeField] private int fadeTriggerLineIndex = 5;

    [Header("Referências de Movimento")]
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private float walkSpeed = 1.5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Coroutine walkCoroutine;

    void Awake()
    {
        if (PlayerPrefs.GetInt(sceneCompletionFlag, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(StartSceneSequence());
    }

    private void OnEnable() 
    { 
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueLineStart += HandleDialogueLineStart;
        }
    }

    private void OnDisable() 
    { 
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueLineStart -= HandleDialogueLineStart;
        }
    }

    private IEnumerator StartSceneSequence()
    {
        yield return new WaitForSeconds(delayToStart);
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(sceneDialogue);
        }
    }

    private void HandleDialogueLineStart(int lineIndex)
    {
        if (lineIndex == walkTriggerLineIndex)
        {
            if (walkCoroutine != null) StopCoroutine(walkCoroutine);
            walkCoroutine = StartCoroutine(WalkToDestination());
        }
        else if (lineIndex == fadeTriggerLineIndex)
        {
            if (walkCoroutine != null) StopCoroutine(walkCoroutine);
            if (animator != null) animator.SetInteger("MovementState", 0);
            StartCoroutine(FadeOutAndDeactivate());
        }
    }

    private IEnumerator WalkToDestination()
    {
        if (destinationPoint == null || animator == null) yield break;

        while (Vector3.Distance(transform.position, destinationPoint.position) > 0.1f)
        {
            Vector3 direction = (destinationPoint.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, destinationPoint.position, walkSpeed * Time.deltaTime);
            animator.SetInteger("MovementState", 5);
            if (spriteRenderer != null) spriteRenderer.flipX = direction.x < 0;
            yield return null;
        }
        transform.position = destinationPoint.position;
        animator.SetInteger("MovementState", 0);
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        if (spriteRenderer == null) 
        {
            gameObject.SetActive(false);
            yield break;
        }

        float duration = 2.0f;
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        PlayerPrefs.SetInt(sceneCompletionFlag, 1);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
    }
}