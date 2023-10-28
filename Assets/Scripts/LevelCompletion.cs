using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] private PlayerRanking playerRanking;
    [SerializeField] private Timer gameTimer;

    public void OnLevelComplete()
    {
        gameTimer.StopTimer();
        playerRanking.CalculateRank();
    }
}
