using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Para controlar o texto de instrução
using TMPro; // Se usar TextMeshPro

public class StargazingController : MonoBehaviour
{
    private enum SceneState { Intro, WaitingForPlayer, Stargazing, Finished }
    private SceneState currentState = SceneState.Intro;

    [Header("Identificador de Estado")]
    [SerializeField] private string sceneCompletionFlag = "StargazingSceneCompleted";

    [Header("Referências de Personagens")]
    [SerializeField] private GameObject aylaStanding;
    [SerializeField] private GameObject aylaLyingDown;
    [SerializeField] private GameObject playerStanding;
    [SerializeField] private GameObject playerLyingDown;

    [Header("Referências de Interação")]
    [SerializeField] private GameObject lieDownZone; // A ZonaDeitar com o Collider
    
    [Header("Referências de Câmera/Visão")]
    [SerializeField] private Camera charactersCamera; // Câmera principal
    [SerializeField] private Camera skyCamera; // Câmera do céu

    [Header("Referências de UI")]
    [SerializeField] private TextMeshProUGUI instructionText; // "Aperte E para escolher onde olhar"

    [Header("Diálogos da Cena")]
    [SerializeField] private DialogueData introDialogue; // "Pronto, chegamos!..."
    [SerializeField] private DialogueData stargazingDialogue; // Diálogo longo sobre as estrelas

    void Awake()
    {
        // Lógica de Persistência
        if (PlayerPrefs.GetInt(sceneCompletionFlag, 0) == 1)
        {
            Debug.Log("Cena de observação já concluída. Desativando controller.");
            // Aqui você pode configurar o estado da cena para quando o jogador retorna
            // Ex: Deixar Ayla e Player desativados.
            if(aylaStanding != null) aylaStanding.SetActive(false);
            if(aylaLyingDown != null) aylaLyingDown.SetActive(false);
            gameObject.SetActive(false); // Desativa o próprio controller
            return;
        }
    }

    void Start()
    {
        // Configuração inicial da cena
        if(aylaLyingDown != null) aylaLyingDown.SetActive(false);
        if(playerLyingDown != null) playerLyingDown.SetActive(false);
        if(lieDownZone != null) lieDownZone.SetActive(false); // Começa desativada
        if(skyCamera != null) skyCamera.gameObject.SetActive(false);
        if(charactersCamera != null) charactersCamera.gameObject.SetActive(true);
        if(instructionText != null) instructionText.gameObject.SetActive(false);
        
        // Inicia a primeira fala da Ayla
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        // Trava o movimento do jogador durante a introdução
        if(playerStanding != null) playerStanding.GetComponent<PlayerController>()?.DisableMovement();

        // Inicia o primeiro diálogo
        DialogueManager.Instance.StartDialogue(introDialogue, true); // Modo automático

        // Espera o diálogo terminar (precisamos de um evento ou de uma checagem)
        // Por simplicidade, vamos esperar um tempo fixo por enquanto.
        // O ideal seria usar DialogueManager.OnDialogueEnd
        yield return new WaitForSeconds(5f); // Ajuste este tempo para a duração da fala

        Debug.Log("Ayla deitou.");
        if(aylaStanding != null) aylaStanding.SetActive(false);
        if(aylaLyingDown != null) aylaLyingDown.SetActive(true);
        
        // Libera o jogador e ativa a zona para deitar
        if(playerStanding != null) playerStanding.GetComponent<PlayerController>()?.EnableMovement();
        if(lieDownZone != null) lieDownZone.SetActive(true);

        currentState = SceneState.WaitingForPlayer;
    }

    // Os outros métodos (Update, PlayerLiesDown, etc.) virão nos próximos passos.
}