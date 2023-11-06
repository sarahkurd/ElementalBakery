using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRanking : MonoBehaviour
{
    private LevelManager levelManager; // Reference to the LevelManager script
    private TextMeshProUGUI rankText; // UI element to display rank

    private void Start()
    {
        // Using FindObjectOfType to find the LevelManager component in the scene
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager is not found in the scene.");
            return;
        }

        // Using FindObjectOfType to find the TextMeshProUGUI component in the scene
        rankText = FindObjectOfType<TextMeshProUGUI>();
        if (rankText == null)
        {
            Debug.LogError("RankText is not found in the scene.");
            return;
        }
    }

    // This method should be called when you want to update the rank text on the UI.
    public void DisplayRank()
    {
        // Check is redundant if you're already doing this in Start, but added for safety.
        if (levelManager == null || rankText == null)
        {
            Debug.LogError("Dependencies are not set in PlayerRanking.");
            return;
        }

        // Read the rank from the LevelManager script
        string rank = levelManager.PlayerRank; // Assuming there's a public property for playerRank in LevelManager

        // Update the UI element with the rank
        rankText.text = "Rank: " + rank;
    }
}
