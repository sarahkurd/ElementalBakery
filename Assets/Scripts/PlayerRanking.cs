using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRanking : MonoBehaviour
{
    [SerializeField] private Timer gameTimer;
    [SerializeField] TextMeshProUGUI rankText;

    // Define rank thresholds (in seconds)
    [SerializeField] private float rankSThreshold = 30f, rankAThreshold = 50f, rankBThreshold = 80f;

    public void CalculateRank()
    {
        float completionTime = gameTimer.TimeUsed;
        Debug.Log("Completion Time: " + completionTime);

        string rank = "You meow out of time";
        if (completionTime <= rankSThreshold) rank = "Master Chef!!!";
        else if (completionTime <= rankAThreshold) rank = "Great Chef!!";
        else if (completionTime <= rankBThreshold) rank = "Novice Chef!";

        rankText.text = "Rank: " + rank;
    }
}
