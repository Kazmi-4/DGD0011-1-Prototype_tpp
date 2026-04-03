using UnityEngine;

// This creates the actual right-click menu option for our coin list!
[CreateAssetMenu(fileName = "New Collectible Set", menuName = "Parkour System/Sets/Collectible Set")]
public class CollectibleSet : RuntimeSet<Collectible>
{
    // It's completely empty inside because the RuntimeSet base class does all the hard work!
}