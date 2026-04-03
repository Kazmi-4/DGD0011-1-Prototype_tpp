using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Parkour System/Game Event")]
public class GameEvent : ScriptableObject
{
    // A list of everyone currently tuned in to this radio station
    private List<GameEventListener> listeners = new List<GameEventListener>();

    // This is the "Broadcast" button! Anything can call this.
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener)) listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener)) listeners.Remove(listener);
    }
}