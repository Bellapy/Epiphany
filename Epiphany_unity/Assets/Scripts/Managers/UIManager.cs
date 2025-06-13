using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Referências da UI de Reflexão")]
    [SerializeField] private CanvasGroup reflectionPanelCanvasGroup;
    [SerializeField] private TextMeshProUGUI reflectionText;
    
    [Header("Configurações de Timing")]
    [SerializeField] private float timePerCharacter = 0.05f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float timeOnScreen = 4.0f; // Tempo que cada frase fica visível

    private Coroutine currentReflectionCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    private void Start()
    {
        if (reflectionPanelCanvasGroup != null)
        {
            reflectionPanelCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("O CanvasGroup do painel de reflexão não foi atribuído no UIManager!");
        }
    }

    public void ShowReflection(ReflectionData data)
    {
        if (currentReflectionCoroutine != null) StopCoroutine(currentReflectionCoroutine);
        currentReflectionCoroutine = StartCoroutine(ShowReflectionCoroutine(data));
    }

    // <<< A LÓGICA PRINCIPAL FOI ALTERADA AQUI >>>
    private IEnumerator ShowReflectionCoroutine(ReflectionData data)
    {
        // PASSO 1: Fade-in do painel de fundo (uma vez só)
        yield return StartCoroutine(FadeCanvasGroup(1f));

        // PASSO 2: Loop para digitar cada frase
        foreach (string line in data.reflectionLines)
        {
            // Digita a frase atual
            yield return StartCoroutine(TypeSentence(line));
            
            // Espera um tempo com a frase já na tela
            yield return new WaitForSeconds(timeOnScreen);
        }

        // PASSO 3: Fade-out do painel de fundo (uma vez só, no final)
        yield return StartCoroutine(FadeCanvasGroup(0f));
    }

    // A coroutine de digitação agora só se preocupa com o texto
    private IEnumerator TypeSentence(string sentence)
    {
        reflectionText.text = ""; // Limpa o texto para a nova frase
        foreach (char letter in sentence.ToCharArray())
        {
            reflectionText.text += letter;
            yield return new WaitForSeconds(timePerCharacter);
        }
    }
    
    // A coroutine de fade agora só controla o Canvas Group
    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        if (reflectionPanelCanvasGroup == null) yield break;

        float startAlpha = reflectionPanelCanvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            reflectionPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        reflectionPanelCanvasGroup.alpha = targetAlpha;
    }
}