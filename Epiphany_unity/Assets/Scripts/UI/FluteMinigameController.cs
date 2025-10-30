using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

// Estrutura para definir uma nota
[System.Serializable]
public class FluteNote
{
    public List<int> requiredHoles; // Índices dos buracos (ex: 0, 2, 4 para buracos 1, 3, 5)
}

public class FluteMinigameController : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private List<Button> holeButtons;
    [SerializeField] private List<Image> filledIndicators;
    [SerializeField] private Button verifyButton;
    [SerializeField] private Button listenAgainButton;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Eventos")]
    public UnityEvent OnMinigameCompleted;

    private List<FluteNote> sequenceToPlay;
    private List<int> playerAttempt = new List<int>();
    private int currentNoteIndex = 0;

    void Start()
    {
        // Adiciona listeners para os botões
        for (int i = 0; i < holeButtons.Count; i++)
        {
            int index = i; // Captura o índice para o delegate
            holeButtons[i].onClick.AddListener(() => OnHoleClicked(index));
        }
        verifyButton.onClick.AddListener(OnVerifyClicked);
        listenAgainButton.onClick.AddListener(OnListenAgainClicked);
    }

    public void StartMinigame(List<FluteNote> sequence)
    {
        sequenceToPlay = sequence;
        currentNoteIndex = 0;
        playerAttempt.Clear();
        UpdateProgressBar();
        
        gameObject.SetActive(true);
        panelCanvasGroup.alpha = 1;
        panelCanvasGroup.interactable = true;

        StartCoroutine(DemonstrationRoutine());
    }

    private IEnumerator DemonstrationRoutine()
    {
        SetInteractable(false);
        messageText.text = "Preste atenção na melodia!";
        yield return new WaitForSeconds(1.5f);

        foreach (var note in sequenceToPlay)
        {
            yield return StartCoroutine(ShowNoteVisual(note, 1.0f));
            yield return new WaitForSeconds(1.0f);
        }

        messageText.text = "Sua vez! Faça a nota e toque no botão para verificar.";
        SetInteractable(true);
    }

    private IEnumerator ShowNoteVisual(FluteNote note, float duration)
    {
        // Fade In
        foreach (int holeIndex in note.requiredHoles)
        {
            StartCoroutine(FadeIndicator(filledIndicators[holeIndex], 1f, 0.3f));
        }
        yield return new WaitForSeconds(duration);

        // Fade Out
        foreach (int holeIndex in note.requiredHoles)
        {
            StartCoroutine(FadeIndicator(filledIndicators[holeIndex], 0f, 0.3f));
        }
    }
    
    private IEnumerator FadeIndicator(Image indicator, float targetAlpha, float duration)
    {
        indicator.gameObject.SetActive(true);
        float startAlpha = indicator.color.a;
        float timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            var color = indicator.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, timer/duration);
            indicator.color = color;
            yield return null;
        }
        if(targetAlpha == 0) indicator.gameObject.SetActive(false);
    }

    private void OnHoleClicked(int index)
    {
        if (playerAttempt.Contains(index))
        {
            playerAttempt.Remove(index);
            filledIndicators[index].gameObject.SetActive(false);
        }
        else
        {
            playerAttempt.Add(index);
            filledIndicators[index].gameObject.SetActive(true);
            var color = filledIndicators[index].color;
            color.a = 1;
            filledIndicators[index].color = color;
        }
    }

    private void OnVerifyClicked()
    {
        FluteNote correctNote = sequenceToPlay[currentNoteIndex];
        playerAttempt.Sort();
        correctNote.requiredHoles.Sort();

        bool isCorrect = new HashSet<int>(playerAttempt).SetEquals(correctNote.requiredHoles);

        if (isCorrect)
        {
            StartCoroutine(FeedbackRoutine(Color.green));
            currentNoteIndex++;
            UpdateProgressBar();

            if (currentNoteIndex >= sequenceToPlay.Count)
            {
                // Minigame concluído!
                StartCoroutine(CompleteMinigame());
            }
        }
        else
        {
            StartCoroutine(FeedbackRoutine(Color.red));
            messageText.text = "Tente de novo!";
        }
    }
    
    private void OnListenAgainClicked()
    {
        StartCoroutine(DemonstrationRoutine());
    }

    private void ResetPlayerAttempt()
    {
        foreach (int index in playerAttempt)
        {
            filledIndicators[index].gameObject.SetActive(false);
        }
        playerAttempt.Clear();
    }

    private IEnumerator FeedbackRoutine(Color color)
    {
        var originalColor = verifyButton.image.color;
        verifyButton.image.color = color;
        yield return new WaitForSeconds(0.5f);
        verifyButton.image.color = originalColor;
        ResetPlayerAttempt();
    }
    
    private IEnumerator CompleteMinigame()
    {
        SetInteractable(false);
        messageText.text = "Perfeito!";
        yield return new WaitForSeconds(1.5f);
        
        panelCanvasGroup.alpha = 0;
        panelCanvasGroup.interactable = false;
        gameObject.SetActive(false);
        
        OnMinigameCompleted.Invoke();
    }

    private void UpdateProgressBar()
    {
        progressBar.value = (float)currentNoteIndex / sequenceToPlay.Count;
    }

    private void SetInteractable(bool interactable)
    {
        foreach (var button in holeButtons) button.interactable = interactable;
        verifyButton.interactable = interactable;
        listenAgainButton.interactable = interactable;
    }
}