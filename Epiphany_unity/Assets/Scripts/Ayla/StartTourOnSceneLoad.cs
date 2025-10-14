using UnityEngine;
[RequireComponent(typeof(NPCTourGuide))]
public class StartTourOnSceneLoad : MonoBehaviour
{
    void Start()
    {
        GetComponent<NPCTourGuide>().StartTour();
    }
}