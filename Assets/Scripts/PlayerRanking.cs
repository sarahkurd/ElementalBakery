using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRanking : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager; // Reference to the LevelManager script
    [SerializeField] private TextMeshProUGUI rankText; // UI element to display rank

    // This method should be called when you want to update the rank text on the UI.
    public void DisplayRank()
    {
        // Ensure that levelManager is not null before trying to access playerRank
        if (levelManager == null)
        {
            Debug.LogError("LevelManager is not set in PlayerRanking.");
            return;
        }

        // Ensure that rankText is not null before trying to update the text
        if (rankText == null)
        {
            Debug.LogError("RankText is not set in PlayerRanking.");
            return;
        }

        // Read the rank from the LevelManager script
        string rank = levelManager.PlayerRank; // Assuming there's a public property for playerRank in LevelManager

        // Update the UI element with the rank
        rankText.text = "Rank: " + rank;
    }
}
