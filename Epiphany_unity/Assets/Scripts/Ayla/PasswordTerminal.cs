using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;

public class PasswordTerminal : MonoBehaviour, IInteractable
{
    [Header("Configuração do Puzzle")]
    [SerializeField] private string correctPassword = "gatomolhado";
    [SerializeField] private int passwordLength = 11;
    [SerializeField] private string sceneToLoadOnSuccess = "vila1";

    [Header("Referências de UI")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private TextMeshProUGUI passwordInputText;
    [SerializeField] private CanvasGroup passwordPanelCanvasGroup;

    // <<< NOVAS LINHAS ADICIONADAS AQUI >>>
    [Header("Referências de Transição")]
    [Tooltip("Arraste aqui o painel de UI com uma imagem branca e um CanvasGroup.")]
    [SerializeField] private CanvasGroup whiteFadePanel;
    [SerializeField] private float fadeDuration = 1.5f;
    // <<< FIM DA ADIÇÃO >>>

    private StringBuilder currentInput;
    private bool isPanelOpen = false;

    void Awake()
    {
        currentInput = new StringBuilder(passwordLength);
        if (passwordPanel != null)
        {
            passwordPanel.SetActive(false);
        }
        if (passwordPanel != null && passwordPanelCanvasGroup == null)
        {
            passwordPanelCanvasGroup = passwordPanel.GetComponent<CanvasGroup>();
        }
        
        // Garante que o painel de fade comece invisível
        if (whiteFadePanel != null)
        {
            whiteFadePanel.gameObject.SetActive(false);
            whiteFadePanel.alpha = 0;
        }
    }

    // O método Start() não precisa mais encontrar o FadeController.

    public void Interact()
    {
        if (isPanelOpen) return;
        OpenPanel();
    }

    private void OpenPanel()
    {
        passwordPanel.SetActive(true);
        if (passwordPanelCanvasGroup != null) passwordPanelCanvasGroup.alpha = 1f;
        currentInput.Clear();
        UpdatePasswordDisplay();
        
        FindFirstObjectByType<PlayerController>()?.DisableMovement();
        
        StartCoroutine(EnablePanelAfterFrame());
    }

    private IEnumerator EnablePanelAfterFrame()
    {
        yield return null; 
        isPanelOpen = true;
    }

    private void ClosePanel()
    {
        isPanelOpen = false;
        passwordPanel.SetActive(false);
        
        FindFirstObjectByType<PlayerController>()?.EnableMovement();
    }

    void Update()
    {
        if (!isPanelOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            currentInput.Length--; 
            UpdatePasswordDisplay();
        }

        if (Input.anyKeyDown && currentInput.Length < passwordLength)
        {
            string input = Input.inputString;
            if (!string.IsNullOrEmpty(input) && char.IsLetter(input[0]))
            {
                currentInput.Append(char.ToLower(input[0]));
                UpdatePasswordDisplay();

                if (currentInput.Length == passwordLength)
                {
                    CheckPassword();
                }
            }
        }
    }

    private void UpdatePasswordDisplay()
    {
        StringBuilder displayText = new StringBuilder();
        for (int i = 0; i < passwordLength; i++)
        {
            if (i < currentInput.Length)
            {
                displayText.Append(currentInput[i]);
            }
            else
            {
                displayText.Append('_');
            }
            displayText.Append(' ');
        }
        passwordInputText.text = displayText.ToString().ToUpper();
    }

    // <<< FUNÇÃO CHECKPASSWORD MODIFICADA >>>
    private void CheckPassword()
    {
        if (currentInput.ToString() == correctPassword)
        {
            isPanelOpen = false;
            // Inicia a sequência de transição completa
            StartCoroutine(SuccessSequence());
        }
        else
        {
            StartCoroutine(IncorrectPasswordRoutine());
        }
    }

    // <<< NOVA CORROTINA ADICIONADA AQUI >>>
    private IEnumerator SuccessSequence()
    {
        // 1. Faz o fade out do painel da senha
        if (passwordPanelCanvasGroup != null)
        {
            float panelFadeDuration = 0.5f;
            float elapsedTime = 0f;
            while (elapsedTime < panelFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                passwordPanelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / panelFadeDuration);
                yield return null;
            }
            passwordPanel.SetActive(false);
        }

        // 2. Faz o fade in do painel branco
        if (whiteFadePanel != null)
        {
            whiteFadePanel.gameObject.SetActive(true);
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                whiteFadePanel.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            whiteFadePanel.alpha = 1f;
        }

        // 3. Carrega a próxima cena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(sceneToLoadOnSuccess);
        }
        else
        {
            Debug.LogError("[PasswordTerminal] ERRO CRÍTICO: GameManager.Instance é NULO! Não é possível carregar a cena.");
        }
    }
    
    private IEnumerator IncorrectPasswordRoutine()
    {
        isPanelOpen = false;
        passwordInputText.color = Color.red;
        yield return new WaitForSeconds(0.75f);
        passwordInputText.color = Color.white;
        currentInput.Clear();
        UpdatePasswordDisplay();
        isPanelOpen = true;
    }
}