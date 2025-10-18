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

    private bool isViewingSky = false;
    private bool canSwitchCamera = false;
    private Animator aylaAnimator;

    void Awake()
{
    if (PlayerPrefs.GetInt(sceneCompletionFlag, 0) == 1)
    {
        Debug.Log("Cena de observação já concluída. Desativando controller e Ayla.");
        
        // Adicione estas linhas para garantir que ela não esteja lá:
        if (aylaStanding != null) aylaStanding.SetActive(false);
        if (aylaLyingDown != null) aylaLyingDown.SetActive(false);
        if (lieDownZone != null) lieDownZone.SetActive(false);

        gameObject.SetActive(false); 
        return;
    }
}

    private void OnEnable() { DialogueManager.OnDialogueEnd += HandleDialogueEnd; }
    private void OnDisable() { DialogueManager.OnDialogueEnd -= HandleDialogueEnd; }

    void Start()
    {
        aylaLyingDown.SetActive(false);
        playerLyingDown.SetActive(false);
        lieDownZone.SetActive(false);
        skyCamera.gameObject.SetActive(false);
        charactersCamera.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(false);
        
        if (aylaStanding != null)
        {
            aylaAnimator = aylaStanding.GetComponent<Animator>();
        }
        
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        currentState = SceneState.Intro;
        
        yield return null; 
        
        DialogueManager.Instance.StartDialogue(introDialogue, true);

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

    private void HandleDialogueEnd()
    {
        if (currentState == SceneState.Intro)
        {
            lieDownZone.SetActive(true);
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

        playerStanding.SetActive(false);
        playerLyingDown.SetActive(true);
        
        instructionText.gameObject.SetActive(true);
        
        DialogueManager.Instance.StartDialogue(stargazingDialogue); 

        // CORREÇÃO DO INPUT DUPLO: Habilita a troca de câmera em uma corrotina
        StartCoroutine(EnableCameraSwitchAfterDelay());
    }

    // NOVA CORROTINA para resolver o input duplo
    private IEnumerator EnableCameraSwitchAfterDelay()
    {
        // Espera pelo final do frame atual.
        // Isso garante que o Input.GetKeyDown(KeyCode.E) do frame atual já tenha sido processado.
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

        // A lógica de troca de câmera agora depende do 'canSwitchCamera'
        if (canSwitchCamera && Input.GetKeyDown(KeyCode.E))
        {
            isViewingSky = !isViewingSky;
            skyCamera.gameObject.SetActive(isViewingSky);
            charactersCamera.gameObject.SetActive(!isViewingSky);
        }
    }
    
    private IEnumerator EndSceneSequence()
{
    instructionText.gameObject.SetActive(false);
    yield return new WaitForSeconds(3.0f);

    Debug.Log("Iniciando fade-out e transição de cena.");
    if (FadeController.Instance != null)
    {
        PlayerPrefs.SetInt(sceneCompletionFlag, 1);
        PlayerPrefs.Save();
        
        // --- LÓGICA DE TRANSIÇÃO AQUI ---
        FadeController.Instance.StartFadeOut(() => {
            // 1. Define o ponto de spawn na próxima cena.
            

            // 2. Carrega a cena do quarto de visitas.
            GameManager.Instance.LoadScene("quarto2"); // <<< SUBSTITUA PELO NOME REAL
        });
        // --- FIM DA LÓGICA DE TRANSIÇÃO ---
    }
}
}