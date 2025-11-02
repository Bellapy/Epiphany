using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class StoryStationController : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private CanvasGroup stationCanvasGroup;
    [SerializeField] private StaticTypewriter typewriter;

    [Header("Conteúdo da História")]
    [TextArea(3, 10)]
    [SerializeField] private List<string> textFragments;

    [Header("Eventos")]
    public UnityEvent OnStationCompleted;

    private int currentFragmentIndex = 0;
    private bool isStationActive = false;
    private bool isTyping = false;
    private PlayerController playerController;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        stationCanvasGroup.alpha = 0;
        stationCanvasGroup.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isStationActive || !other.CompareTag("Player")) return;

        playerController = other.GetComponent<PlayerController>();
        isStationActive = true;
        StartCoroutine(ShowStation());
    }

    private void Update()
    {
        if (!isStationActive) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                // --- LÓGICA DE ACELERAR TEXTO ---
                // Se está digitando, pula para o final.
                typewriter.SkipToEnd();
            }
            else
            {
                // Se já terminou de digitar, avança para o próximo fragmento.
                currentFragmentIndex++;
                if (currentFragmentIndex < textFragments.Count)
                {
                    typewriter.StartTyping(textFragments[currentFragmentIndex]);
                }
                else
                {
                    // Todos os fragmentos foram lidos
                    StartCoroutine(HideStation());
                }
            }
        }
    }

    private IEnumerator ShowStation()
    {
        // --- LÓGICA DE BLOQUEIO DE MOVIMENTO ---
        if (playerController != null)
        {
            playerController.DisableMovement();
        }
        
        stationCanvasGroup.gameObject.SetActive(true);
        
        // --- LÓGICA DE FADE-IN ---
        yield return StartCoroutine(FadeCanvasGroup(1f, 0.5f));
        
        currentFragmentIndex = 0;
        typewriter.StartTyping(textFragments[currentFragmentIndex]);
    }

    private IEnumerator HideStation()
    {
        isStationActive = false;
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(FadeCanvasGroup(0f, 0.5f));
        stationCanvasGroup.gameObject.SetActive(false);
        
        // --- LÓGICA DE RESTAURAR MOVIMENTO ---
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
        
        OnStationCompleted.Invoke();
        gameObject.SetActive(false);
    }

    public void OnTypingStateChanged(bool typingStatus)
    {
        // Esta função será chamada pelo StaticTypewriter
        isTyping = typingStatus;
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = stationCanvasGroup.alpha;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            stationCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        stationCanvasGroup.alpha = targetAlpha;
    }
}