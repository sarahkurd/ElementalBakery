using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRanking : MonoBehaviour
{
    [SerializeField] private Timer gameTimer;
    [SerializeField] private TextMeshProUGUI rankText;

    // Define rank thresholds (in seconds)
    [SerializeField] private float rankSThreshold = 30f, rankAThreshold = 50f, rankBThreshold = 80f;

    public void CalculateRank()
    {
        // Ensure that gameTimer is not null before trying to access TimeUsed
        if (gameTimer == null)
        {
            Debug.LogError("GameTimer is not set in PlayerRanking.");
            return;
        }

        float completionTime = gameTimer.TimeUsed;
        Debug.Log("Completion Time: " + completionTime);

        // Ensure that rankText is not null before trying to update the text
        if (rankText == null)
        {
            Debug.LogError("RankText is not set in PlayerRanking.");
            return;
        }

        string rank = "Novice Chef!"; // Default rank if no thresholds are met
        if (completionTime <= rankSThreshold)
        {
            rank = "Master Chef!!!";
        }
        else if (completionTime <= rankAThreshold)
        {
            rank = "Great Chef!!";
        }
        else if (completionTime <= rankBThreshold)
        {
            rank = "Novice Chef!";
        }

        rankText.text = "Rank: " + rank;
    }
}
