using UnityEngine;

[DefaultExecutionOrder(-100)]

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Input EventManager");
        EventManager eventManager = new EventManager();
        Locator<EventManager>.Provide(eventManager);
    }
}
