using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Step 3.4: Advanced Enum")]
    public ItemTypeSO itemType;

    [Header("Step 3.1: Flexible References")]
    public FloatReference pointsGiven;
    public FloatVariable playerScore;

    [Header("Step 3.2: Decoupled Events")]
    public GameEvent onScoreChanged;

    [Header("Step 3.3: Runtime Sets")]
    public CollectibleSet activeCollectiblesList;

    void OnEnable()
    {
        // Add this coin to the global list when it spawns
        if (activeCollectiblesList != null) activeCollectiblesList.Add(this);
    }

    void OnDisable()
    {
        // Remove this coin from the list when it is destroyed
        if (activeCollectiblesList != null) activeCollectiblesList.Remove(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Add points to the global file
            if (playerScore != null) playerScore.ApplyChange(pointsGiven.Value);

            // 2. Broadcast the "Score Changed" radio signal to the UI
            if (onScoreChanged != null) onScoreChanged.Raise();

            // 3. Destroy the coin
            Destroy(gameObject);
        }
    }
}