using UnityEngine;
using System.Collections;
using TMPro;

public class StargazingController : MonoBehaviour
{
    private enum SceneState { Arriving, Intro, WaitingForPlayer, Stargazing, Finished }
    private SceneState currentState = SceneState.Arriving;

    [Header("Identificador de Estado")]
    [SerializeField] private string sceneCompletionFlag = "StargazingSceneCompleted";

    [Header("Referências de Personagens")]
    [SerializeField] private GameObject aylaStanding;
    [SerializeField] private GameObject aylaLyingDown;
    [SerializeField] private Transform aylaLieDownPoint; 
    [SerializeField] private GameObject playerStanding;
    [SerializeField] private GameObject playerLyingDown;

    [Header("Referências de Interação")]
    [SerializeField] private GameObject lieDownZone;
    
    [Header("Referências de Câmera/Visão")]
    [SerializeField] private Camera charactersCamera;
    [SerializeField] private Camera skyCamera;

    [Header("Referências de UI")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Diálogos da Cena")]
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private DialogueData stargazingDialogue;

    [Header("Configurações de Cena")] 
    [SerializeField] private float aylaWalkSpeed = 1.0f;
    [SerializeField] private string nextSceneName = "quarto2";

    private bool isViewingSky = false;
    private bool canSwitchCamera = false;
    private Animator aylaAnimator;
    private FadeController fadeController;

    void Awake()
    {
        if (PlayerPrefs.GetInt(sceneCompletionFlag, 0) == 1)
        {
            if (aylaStanding != null) aylaStanding.SetActive(false);
            if (aylaLyingDown != null) aylaLyingDown.SetActive(false);
            if (lieDownZone != null) lieDownZone.SetActive(false);

            gameObject.SetActive(false); 
            return;
        }
    }

    void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();

        if (aylaLyingDown != null) aylaLyingDown.SetActive(false);
        if (playerLyingDown != null) playerLyingDown.SetActive(false);
        if (lieDownZone != null) lieDownZone.SetActive(false);
        if (skyCamera != null) skyCamera.gameObject.SetActive(false);
        if (charactersCamera != null) charactersCamera.gameObject.SetActive(true);
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        
        if (aylaStanding != null)
        {
            aylaAnimator = aylaStanding.GetComponent<Animator>();
        }
        
        StartCoroutine(IntroSequence());
    }

    private void OnEnable() 
    { 
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd; 
        }
    }

    private void OnDisable() 
    { 
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd; 
        }
    }

    private IEnumerator IntroSequence()
    {
        currentState = SceneState.Intro;
        
        yield return null; 
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(introDialogue, true);
        }

        if (aylaStanding != null && aylaLieDownPoint != null)
        {
            SpriteRenderer aylaSprite = aylaStanding.GetComponent<SpriteRenderer>();

            while (Vector3.Distance(aylaStanding.transform.position, aylaLieDownPoint.position) > 0.1f)
            {
                Vector3 direction = (aylaLieDownPoint.position - aylaStanding.transform.position).normalized;
                aylaStanding.transform.position = Vector3.MoveTowards(aylaStanding.transform.position, aylaLieDownPoint.position, aylaWalkSpeed * Time.deltaTime);

                if (aylaAnimator != null) aylaAnimator.SetInteger("MovementState", 5); 
                if (aylaSprite != null) aylaSprite.flipX = direction.x < 0;

                yield return null; 
            }

            aylaStanding.transform.position = aylaLieDownPoint.position;
        }
    }

    private void HandleDialogueEnd()
    {
        if (currentState == SceneState.Intro)
        {
            if (lieDownZone != null) lieDownZone.SetActive(true);
            currentState = SceneState.WaitingForPlayer;
        }
        else if (currentState == SceneState.Stargazing)
        {
            currentState = SceneState.Finished;
            StartCoroutine(EndSceneSequence());
        }
    }

    public void PlayerLiesDown()
    {
        if (currentState != SceneState.WaitingForPlayer) return;
        
        currentState = SceneState.Stargazing;

        if (playerStanding != null) playerStanding.SetActive(false);
        if (playerLyingDown != null) playerLyingDown.SetActive(true);
        
        if (instructionText != null) instructionText.gameObject.SetActive(true);
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(stargazingDialogue); 
        }

        StartCoroutine(EnableCameraSwitchAfterDelay());
    }

    private IEnumerator EnableCameraSwitchAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        canSwitchCamera = true;
    }

    void Update()
    {
        if (currentState == SceneState.WaitingForPlayer)
        {
            if (aylaAnimator != null)
            {
                aylaAnimator.SetInteger("MovementState", 0);
            }
        }

        if (canSwitchCamera && Input.GetKeyDown(KeyCode.E))
        {
            isViewingSky = !isViewingSky;
            if (skyCamera != null) skyCamera.gameObject.SetActive(isViewingSky);
            if (charactersCamera != null) charactersCamera.gameObject.SetActive(!isViewingSky);
        }
    }
    
    private IEnumerator EndSceneSequence()
    {
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        yield return new WaitForSeconds(3.0f);
        
        PlayerPrefs.SetInt(sceneCompletionFlag, 1);
        PlayerPrefs.Save();

        if (fadeController != null && GameManager.Instance != null)
        {
            fadeController.StartFadeOut(() => {
                GameManager.Instance.LoadScene(nextSceneName);
            });
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(nextSceneName);
        }
    }
}