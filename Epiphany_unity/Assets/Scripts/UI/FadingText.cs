using UnityEngine;
using TMPro;
using System.Collections;

public class FadingText : MonoBehaviour
{
    [Header("Configurações do Efeito")]
    [Tooltip("A intensidade do tremor. Valores maiores = tremor mais forte.")]
    [SerializeField] private float trembleMagnitude = 1.5f;

    private TextMeshProUGUI textField;
    private RectTransform rectTransform;
    private Coroutine lifecycleCoroutine;

    private void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartLifecycle(string text, float typeSpeed, float fadeInTime, float visibleTime, float fadeOutTime, float finalAlpha)
    {
        if (lifecycleCoroutine != null) StopCoroutine(lifecycleCoroutine);
        lifecycleCoroutine = StartCoroutine(LifecycleRoutine(text, typeSpeed, fadeInTime, visibleTime, fadeOutTime, finalAlpha));
    }

    private IEnumerator LifecycleRoutine(string text, float typeSpeed, float fadeInTime, float visibleTime, float fadeOutTime, float finalAlpha)
    {
        textField.alpha = 0;
        textField.text = "";

        yield return Fade(1f, fadeInTime);

        foreach (char letter in text.ToCharArray())
        {
            textField.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        StartCoroutine(TrembleRoutine());

        yield return new WaitForSeconds(visibleTime);
        yield return Fade(finalAlpha, fadeOutTime);
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = textField.alpha;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            textField.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        textField.alpha = targetAlpha;
    }

    private IEnumerator TrembleRoutine()
    {
        Vector2 originalPos = rectTransform.anchoredPosition;
        while (textField.alpha > 0.1f) 
        {
            float x = Random.Range(-1f, 1f) * trembleMagnitude;
            float y = Random.Range(-1f, 1f) * trembleMagnitude;
            rectTransform.anchoredPosition = originalPos + new Vector2(x, y);
            yield return new WaitForSeconds(0.05f);
        }
        rectTransform.anchoredPosition = originalPos;
    }
}