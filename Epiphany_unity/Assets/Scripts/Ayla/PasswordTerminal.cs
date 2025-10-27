using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;

public class PasswordTerminal : MonoBehaviour, IInteractable
{
    [Header("Configuração do Puzzle")]
    [SerializeField] private string correctPassword = "gatomolhado";
    [SerializeField] private int passwordLength = 11;

    [Header("Referências de UI")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private TextMeshProUGUI passwordInputText;
    [SerializeField] private CanvasGroup passwordPanelCanvasGroup;

    [Header("Referências de Efeitos")]
    [Tooltip("Arraste o Prefab do efeito de partículas de teletransporte aqui.")]
    [SerializeField] private GameObject teleportEffectPrefab;

    private StringBuilder currentInput;
    private bool isPanelOpen = false;
    private FadeController fadeController;

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
    }

    void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();
    }

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

    private void CheckPassword()
    {
        if (currentInput.ToString() == correctPassword)
        {
            isPanelOpen = false;

            if (teleportEffectPrefab != null)
            {
                PlayerController player = FindFirstObjectByType<PlayerController>();
                if (player != null)
                {
                    Instantiate(teleportEffectPrefab, player.transform.position, Quaternion.identity);
                }
            }
            
            StartCoroutine(FadeOutPasswordPanel());

            if (fadeController != null && GameManager.Instance != null)
            {
                fadeController.StartFadeOut(() => {
                    GameManager.Instance.LoadScene("zric");
                }, Color.white);
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadScene("zric");
            }
        }
        else
        {
            StartCoroutine(IncorrectPasswordRoutine());
        }
    }
    
    private IEnumerator FadeOutPasswordPanel()
    {
        if (passwordPanelCanvasGroup == null || fadeController == null) yield break;

        float duration = fadeController.fadeDuration;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            passwordPanelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }

        passwordPanel.SetActive(false);
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