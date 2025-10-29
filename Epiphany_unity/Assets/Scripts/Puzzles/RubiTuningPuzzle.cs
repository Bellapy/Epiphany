using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class RubiTuningPuzzle : MonoBehaviour, IInteractable
{
    [Header("Referências de UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
     [Header("Gerenciamento de Estado")]
    [SerializeField] private string completionFlag = "ZricSceneCompleted";
    [SerializeField] private Slider blueSlider;
    [SerializeField] private Image colorPreview;
    [SerializeField] private Image targetColorPreview;
    [SerializeField] private Button checkButton; // Referência para o botão de confirmação

    [Header("Referências de UI")]

    [Header("Configuração do Puzzle")]
    [SerializeField] private Color targetColor;
    [SerializeField] private float tolerance = 0.15f;

    [Header("Eventos")]
    public UnityEvent OnPuzzleSolved;

    void Start()
    {
        // Configura os listeners para os sliders atualizarem a cor em tempo real
        if (redSlider != null) redSlider.onValueChanged.AddListener(delegate { UpdateColorPreview(); });
        if (greenSlider != null) greenSlider.onValueChanged.AddListener(delegate { UpdateColorPreview(); });
        if (blueSlider != null) blueSlider.onValueChanged.AddListener(delegate { UpdateColorPreview(); });
        
        // Configura o listener para o botão de verificação
        if (checkButton != null) checkButton.onClick.AddListener(CheckSolutionFromButton);
        
        // Garante que o painel comece desativado
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
    }

    // Método principal de interação, funciona como um interruptor (toggle)
    public void Interact()
    {
        // --- NOVA VERIFICAÇÃO ADICIONADA ---
        // Se a cena já foi concluída, não faz nada.
        if (PlayerPrefs.GetInt(completionFlag, 0) == 1)
        {
            return;
        }
        // --- FIM DA NOVA VERIFICAÇÃO ---

        if (puzzlePanel.activeSelf)
        {
            ClosePuzzle();
        }
        else
        {
            OpenPuzzle();
        }
    }

    private void OpenPuzzle()
    {
        if (puzzlePanel == null)
        {
            Debug.LogError("[RubiTuningPuzzle] A referência para 'puzzlePanel' está NULA! Verifique o Inspector.");
            return;
        }
        
        // Randomiza os sliders para um novo desafio a cada vez
        redSlider.value = Random.Range(0f, 1f);
        greenSlider.value = Random.Range(0f, 1f);
        blueSlider.value = Random.Range(0f, 1f);
        
        // Mostra a cor alvo
        if (targetColorPreview != null)
        {
            targetColorPreview.color = targetColor;
        }

        UpdateColorPreview();
        puzzlePanel.SetActive(true);
    }

    private void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);
    }

    // Atualiza a cor de preview conforme o jogador mexe nos sliders
    private void UpdateColorPreview()
    {
        if (colorPreview == null) return;
        Color currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        colorPreview.color = currentColor;
    }

    // Método chamado exclusivamente pelo botão "Ativar"
    private void CheckSolutionFromButton()
    {
        Color currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        
        bool redMatch = Mathf.Abs(currentColor.r - targetColor.r) <= tolerance;
        bool greenMatch = Mathf.Abs(currentColor.g - targetColor.g) <= tolerance;
        bool blueMatch = Mathf.Abs(currentColor.b - targetColor.b) <= tolerance;

        if (redMatch && greenMatch && blueMatch)
        {
            // Se a solução estiver correta, inicia a sequência de sucesso
            StartCoroutine(SolveSequence());
        }
        else
        {
            // Se a solução estiver errada, dá um feedback visual de erro
            StartCoroutine(IncorrectSolutionFeedback());
        }
    }
    
    // Corrotina para o feedback de solução incorreta (pisca em vermelho)
    private IEnumerator IncorrectSolutionFeedback()
    {
        checkButton.interactable = false; // Desabilita o botão durante o feedback
        colorPreview.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        UpdateColorPreview(); // Volta para a cor que o jogador escolheu
        yield return new WaitForSeconds(0.2f);
        colorPreview.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        UpdateColorPreview();
        checkButton.interactable = true; // Reabilita o botão
    }

    // Corrotina para a sequência de sucesso
    private IEnumerator SolveSequence()
    {
        // 1. Trava todos os controles
        redSlider.interactable = false;
        greenSlider.interactable = false;
        blueSlider.interactable = false;
        checkButton.interactable = false;
        
        // 2. Animação visual de "processamento"
        float processTime = 1.0f;
        float timer = 0f;
        Color correctColor = colorPreview.color;

        while (timer < processTime)
        {
            colorPreview.color = Color.white;
            yield return new WaitForSecondsRealtime(0.1f);
            colorPreview.color = correctColor;
            yield return new WaitForSecondsRealtime(0.1f);
            timer += 0.2f;
        }
        
        // 3. Feedback final de sucesso (verde)
        colorPreview.color = Color.green;
        yield return new WaitForSecondsRealtime(0.75f);

        // 4. Conclusão: fecha o painel e invoca o evento
        ClosePuzzle();
        OnPuzzleSolved.Invoke();
        
        // 5. Reabilita os controles para a próxima vez que o puzzle for aberto
        redSlider.interactable = true;
        greenSlider.interactable = true;
        blueSlider.interactable = true;
        checkButton.interactable = true;
    }
}