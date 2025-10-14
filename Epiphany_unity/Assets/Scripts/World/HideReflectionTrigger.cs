// HideReflectionTrigger.cs
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HideReflectionTrigger : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou na área de esconder reflexão. Chamando HideReflection().");
        }
    }
}