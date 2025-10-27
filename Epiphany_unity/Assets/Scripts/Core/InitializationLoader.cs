using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationLoader : MonoBehaviour
{
    private static bool isInitialized = false;

    void Awake()
    {
        if (isInitialized)
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.LoadScene("Initializer", LoadSceneMode.Additive);
        isInitialized = true;
    }
}