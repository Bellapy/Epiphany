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
    
    [Header("Flags de Evento (Opcional)")]
    [Tooltip("Se preenchido, define este PlayerPrefs flag como '1' (concluído) ao usar a transição.")]
    [SerializeField] private string eventFlagToSetOnTransition;

    private FadeController fadeController;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    
    void Start()
    {
        fadeController = FindFirstObjectByType<FadeController>();
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
        if (GameManager.Instance == null)
        {
            return;
        }

        this.enabled = false;

        if (!string.IsNullOrEmpty(eventFlagToSetOnTransition))
        {
            PlayerPrefs.SetInt(eventFlagToSetOnTransition, 1);
            PlayerPrefs.Save();
        }

        GameManager.Instance.SetNextSpawnPoint(spawnPointNameInNextScene);

        foreach (string objectName in objectsToActivate)
        {
            GameManager.Instance.AddObjectToActivateOnLoad(objectName);
        }

        if (fadeController != null)
        {
            fadeController.StartFadeOut(() => {
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