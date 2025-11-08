using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ConfrontationController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject fadingTextPrefab;
    [SerializeField] private Image whiteExplosionPanel;
    [SerializeField] private TextMeshProUGUI finalMessageText;

    [Header("Configuração de Transição")]
    [SerializeField] private string nextSceneName = "EncontrocomAyla";

    [Header("Conteúdo das Frases")]
    [TextArea(3, 5)]
    [SerializeField] private List<string> phrases;

    [Header("Configurações de Layout")]
    [SerializeField] private float minDistanceBetweenPhrases = 200f;
    [SerializeField] private int minFontSize = 24;
    [SerializeField] private int maxFontSize = 49;
    [SerializeField] private int placementAttempts = 15;

    [Header("Configurações de Timing e Efeitos")]
    [SerializeField] private float timeBetweenPhrases = 3.0f; 
    [SerializeField] private float typeSpeed = 0.1f;
    [SerializeField] private float finalMessageDelay = 1.0f;
    [SerializeField] private float zoomAmount = 1.1f;
    [SerializeField] private float shakeDuration = 2.0f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private float sceneTimer = 0f;
    private float totalDuration;
    private List<Vector2> usedPositions = new List<Vector2>();
    private FadeController fadeController;

    private void Start()
    {
        totalDuration = timeBetweenPhrases * phrases.Count;
        whiteExplosionPanel.gameObject.SetActive(true);
        whiteExplosionPanel.color = Color.black;
        finalMessageText.gameObject.SetActive(false);
        
        if (finalMessageText.GetComponent<FadingText>() == null)
        {
            finalMessageText.gameObject.AddComponent<FadingText>();
        }

        fadeController = FindFirstObjectByType<FadeController>();
        StartCoroutine(SceneRoutine());
    }

    private IEnumerator SceneRoutine()
    {
        int phraseIndex = 0;
        while (phraseIndex < phrases.Count)
        {
            SpawnFadingText(phrases[phraseIndex], sceneTimer / totalDuration);
            phraseIndex++;
            yield return new WaitForSeconds(timeBetweenPhrases);
        }
        
        sceneTimer = totalDuration;

        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(CameraShake());
        yield return FadePanel(whiteExplosionPanel, Color.white, 1.5f);
        
        yield return new WaitForSeconds(finalMessageDelay);

        finalMessageText.gameObject.SetActive(true);
        finalMessageText.GetComponent<FadingText>().StartLifecycle("Encontre Ayla", 0.08f, 0.5f, 2.0f, 1f, 1f);
        
        yield return new WaitForSeconds(2.5f);

        Debug.Log("Fim da cena de Confronto. Iniciando transição...");

        if (fadeController != null)
        {
            fadeController.StartFadeOut(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadScene(nextSceneName);
                }
            }, Color.black);
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(nextSceneName);
        }
    }

    private void SpawnFadingText(string text, float timePercentage)
    {
        GameObject textInstance = Instantiate(fadingTextPrefab, canvasRect);
        TextMeshProUGUI textField = textInstance.GetComponent<TextMeshProUGUI>();
        FadingText fadingText = textInstance.GetComponent<FadingText>();
        Vector2 bestPosition = Vector2.zero;
        float bestMinDistance = -1f;
        if (usedPositions.Count == 0)
        {
            bestPosition = GetRandomPosition();
        }
        else
        {
            for (int i = 0; i < placementAttempts; i++)
            {
                Vector2 currentPosition = GetRandomPosition();
                float currentMinDistance = FindMinimumDistanceToNeighbors(currentPosition);
                if (currentMinDistance > bestMinDistance)
                {
                    bestMinDistance = currentMinDistance;
                    bestPosition = currentPosition;
                }
            }
        }
        usedPositions.Add(bestPosition);
        textField.rectTransform.anchoredPosition = bestPosition;
        textField.fontSize = Random.Range(minFontSize, maxFontSize);
        if (timePercentage > 0.7f) textField.color = Color.red;
        else if (timePercentage > 0.3f) textField.color = (Random.value > 0.5f) ? Color.white : Color.red;
        else textField.color = Color.white;
        textField.alpha = 0;
        fadingText.StartLifecycle(text, typeSpeed, 1.0f, 2.0f, 3.0f, 0.15f);
    }

    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(canvasRect.rect.xMin * 0.7f, canvasRect.rect.xMax * 0.7f);
        float y = Random.Range(canvasRect.rect.yMin * 0.7f, canvasRect.rect.yMax * 0.7f);
        return new Vector2(x, y);
    }

    private float FindMinimumDistanceToNeighbors(Vector2 position)
    {
        float minDistance = float.MaxValue;
        foreach (Vector2 usedPos in usedPositions)
        {
            float distance = Vector2.Distance(position, usedPos);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }
        return minDistance;
    }
    
    void Update()
    {
        if (sceneTimer < totalDuration)
        {
            sceneTimer += Time.deltaTime;
            
            if (mainCamera.orthographic)
            {
                float initialSize = 5f; 
                float zoomProgress = sceneTimer / totalDuration;
                mainCamera.orthographicSize = Mathf.Lerp(initialSize, initialSize / zoomAmount, zoomProgress);
            }
        }
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0.0f;
        while (elapsed < shakeDuration)
        {
            float currentMagnitude = Mathf.Lerp(0, shakeMagnitude, elapsed / shakeDuration);
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;
            mainCamera.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = originalPos;
    }

    private IEnumerator FadePanel(Image panel, Color targetColor, float duration)
    {
        float timer = 0f;
        Color startColor = panel.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            panel.color = Color.Lerp(startColor, targetColor, timer / duration);
            yield return null;
        }
        panel.color = targetColor;
    }
}