using UnityEngine;

// This line is the magic spell! It tells Unity to add a new option to your right-click menu.
[CreateAssetMenu(fileName = "New Item Type", menuName = "Parkour System/Item Type")]
public class ItemTypeSO : ScriptableObject
{
    [TextArea]
    public string description;
}