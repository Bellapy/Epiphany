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

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se a porta estiver trancada, ou se o objeto não for o jogador, não faz nada.
        if (isLocked || !other.CompareTag("Player"))
        {
            return;
        }

        // Se não estiver trancada, inicia a transição.
        StartSceneTransition();
    }

    private void StartSceneTransition()
    {
        // Desativa o próprio componente para evitar múltiplas transições.
        this.enabled = false;

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