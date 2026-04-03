using UnityEngine;
using UnityEngine.Events; // Required for Unity's built-in event responses!

public class GameEventListener : MonoBehaviour
{
    [Tooltip("What radio station are we listening to?")]
    public GameEvent Event;

    [Tooltip("What should we do when we hear the broadcast?")]
    public UnityEvent Response;

    // Turn the radio on
    private void OnEnable()
    {
        if (Event != null) Event.RegisterListener(this);
    }

    // Turn the radio off
    private void OnDisable()
    {
        if (Event != null) Event.UnregisterListener(this);
    }

    // When the station broadcasts, trigger the response!
    public void OnEventRaised()
    {
        Response.Invoke();
    }
}