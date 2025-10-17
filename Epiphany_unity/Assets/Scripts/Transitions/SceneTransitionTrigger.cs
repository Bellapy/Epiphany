// Em SceneTransitionTrigger.cs

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Configuração da Transição")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnPointNameInNextScene;

    [Header("Gerenciamento de Objetos de Cena")]
    [Tooltip("Lista de nomes de GameObjects que devem ser ATIVADOS na próxima cena.")]
    [SerializeField] private List<string> objectsToActivate;

    [Header("Estado da Porta")]
    [Tooltip("Marque se a porta deve começar trancada.")]
    [SerializeField] private bool isLocked = false;
    
    // --- NOVA ADIÇÃO ---
    [Header("Flags de Evento (Opcional)")]
    [Tooltip("Se preenchido, define este PlayerPrefs flag como '1' (concluído) ao usar a transição.")]
    [SerializeField] private string eventFlagToSetOnTransition;
    // --- FIM DA NOVA ADIÇÃO ---

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLocked || !other.CompareTag("Player"))
        {
            return;
        }
        StartSceneTransition();
    }

    private void StartSceneTransition()
    {
        this.enabled = false;

        // --- NOVA ADIÇÃO ---
        // Se um nome de flag foi definido, salva-o como concluído.
        if (!string.IsNullOrEmpty(eventFlagToSetOnTransition))
        {
            PlayerPrefs.SetInt(eventFlagToSetOnTransition, 1);
            PlayerPrefs.Save();
            Debug.Log($"[SceneTransitionTrigger] Flag de evento '{eventFlagToSetOnTransition}' marcada como concluída.");
        }
        // --- FIM DA NOVA ADIÇÃO ---

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager não encontrado! A transição de cena falhou.");
            return;
        }

        GameManager.Instance.SetNextSpawnPoint(spawnPointNameInNextScene);

        foreach (string objectName in objectsToActivate)
        {
            GameManager.Instance.AddObjectToActivateOnLoad(objectName);
        }

        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeOut(() => {
                GameManager.Instance.LoadScene(sceneToLoad);
            });
        }
        else
        {
            GameManager.Instance.LoadScene(sceneToLoad);
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }
}