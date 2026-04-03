using UnityEngine;
using TMPro; // Required for Unity's modern text system

public class ScoreDisplay : MonoBehaviour
{
    [Tooltip("Drag your Global_PlayerScore file here!")]
    public FloatVariable globalPlayerScore;

    private TextMeshProUGUI scoreText;

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScoreText(); // Set it to 0 at the start of the game
    }

    // The GameEventListener will trigger this specific function!
    public void UpdateScoreText()
    {
        if (scoreText != null && globalPlayerScore != null)
        {
            scoreText.text = "Tokens: " + globalPlayerScore.Value.ToString();
        }
    }
}