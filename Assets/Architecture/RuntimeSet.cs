using System.Collections.Generic;
using UnityEngine;

// The <T> means "Type". It makes this a universal template!
public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> Items = new List<T>();

    public void Add(T thing)
    {
        if (!Items.Contains(thing)) Items.Add(thing);
    }

    public void Remove(T thing)
    {
        if (Items.Contains(thing)) Items.Remove(thing);
    }
}