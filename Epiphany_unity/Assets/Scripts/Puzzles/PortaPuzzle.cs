using UnityEngine;
using System.Collections.Generic; // Adicionado para garantir que a List<string> seja reconhecida

public class PortaPuzzle : MonoBehaviour, IInteractable
{
    private bool estaTrancada = true;

    public void Interact()
    {
        if (estaTrancada)
        {
            // <<< A CORREÇÃO ESTÁ AQUI >>>
            // Agora estamos criando uma List<string> em vez de um string[]
            DialogueManager.Instance.StartReflection(new ReflectionData { reflectionLines = new List<string> { "Parece trancada... Acerte a ordem das luzes." } });
        }
        else
        {
            Debug.Log("Porta destrancada! Transição para a próxima cena aqui.");
            // Coloque a sua lógica de DoorTrigger aqui.
        }
    }

    public void Destrancar()
    {
        estaTrancada = false;
        Debug.Log("A porta foi destrancada!");
    }
}