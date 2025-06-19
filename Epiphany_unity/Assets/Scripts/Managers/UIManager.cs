// UIManager.cs
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
    [SerializeField] private float timePerCharacter = 0.02f;
    [SerializeField] private float fadeDuration = 0.5f;
    // Não precisamos mais do "timeOnScreen", pois o texto ficará visível até o próximo comando.

    private Coroutine currentTypingCoroutine;
    private bool isPanelVisible = false; // <<< NOVA VARIÁVEL: Nosso "lembrete" do estado do painel

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
            isPanelVisible = false;
        }
        else
        {
            Debug.LogError("O CanvasGroup do painel de reflexão não foi atribuído no UIManager!");
        }
    }

    // --- MÉTODO PRINCIPAL MODIFICADO ---
    public void ShowReflection(ReflectionData data)
    {
        // Se já estivermos digitando algo, paramos a digitação anterior.
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
        }
        
        // Inicia a nova corrotina para mostrar o texto.
        currentTypingCoroutine = StartCoroutine(ShowReflectionCoroutine(data));
    }

    private IEnumerator ShowReflectionCoroutine(ReflectionData data)
    {
        // PASSO 1: VERIFICAR O ESTADO DO PAINEL
        // Se o painel não estiver visível, fazemos o Fade In primeiro.
        if (!isPanelVisible)
        {
            yield return StartCoroutine(FadeCanvasGroup(1f));
            isPanelVisible = true;
        }

        // PASSO 2: LOOP PARA DIGITAR CADA FRASE (com uma pequena pausa entre elas)
        // Usamos um loop for para iterar pela lista de frases.
        for (int i = 0; i < data.reflectionLines.Count; i++)
        {
            string line = data.reflectionLines[i];
            
            // Digita a frase atual.
            yield return StartCoroutine(TypeSentence(line));
            
            // Adiciona uma pequena pausa antes da próxima frase, se não for a última.
            if (i < data.reflectionLines.Count - 1)
            {
                yield return new WaitForSeconds(1.5f); // Pausa de 1.5s entre as frases da mesma placa
            }
        }
        // NÃO fazemos mais o Fade Out aqui!
    }

    // --- NOVO MÉTODO PARA ESCONDER O PAINEL ---
    /// <summary>
    /// Força o painel de reflexão a desaparecer com um fade out.
    /// </summary>
    public void HideReflection()
    {
        // Só executa se o painel estiver visível.
        if (isPanelVisible)
        {
            if (currentTypingCoroutine != null)
            {
                StopCoroutine(currentTypingCoroutine);
            }
            StartCoroutine(HidePanelCoroutine());
        }
    }

    private IEnumerator HidePanelCoroutine()
    {
        yield return StartCoroutine(FadeCanvasGroup(0f));
        isPanelVisible = false;
    }

    // A coroutine de digitação permanece a mesma.
    private IEnumerator TypeSentence(string sentence)
    {
        reflectionText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            reflectionText.text += letter;
            yield return new WaitForSeconds(timePerCharacter);
        }
    }
    
    // A coroutine de fade permanece a mesma.
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