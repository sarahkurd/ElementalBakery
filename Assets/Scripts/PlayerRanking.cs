using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRanking : MonoBehaviour
{
    [SerializeField] private Timer gameTimer;
    [SerializeField] TextMeshProUGUI rankText;

    // Define rank thresholds (in seconds)
    private float rankSThreshold = 60f;
    private float rankAThreshold = 120f;
    private float rankBThreshold = 180f;

    public void CalculateRank()
    {
        float completionTime = gameTimer.ElapsedTime;

        string rank = "C";
        if (completionTime <= rankSThreshold) rank = "S";
        else if (completionTime <= rankAThreshold) rank = "A";
        else if (completionTime <= rankBThreshold) rank = "B";

        rankText.text = "Rank: " + rank;
    }
}
