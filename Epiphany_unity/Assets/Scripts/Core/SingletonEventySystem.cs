using UnityEngine;
using UnityEngine.EventSystems;

public class SingletonEventSystem : MonoBehaviour
{
    public static SingletonEventSystem Instance { get; private set; }

    void Awake()
    {
        // Se já existe uma instância global deste sistema...
        if (Instance != null && Instance != this)
        {
            // ...então eu sou uma duplicata desnecessária. Adeus!
            Destroy(gameObject);
            return;
        }

        // Se não existe, eu sou o escolhido!
        Instance = this;
        // Me torno imortal e persistente entre cenas.
        DontDestroyOnLoad(gameObject);
    }
}