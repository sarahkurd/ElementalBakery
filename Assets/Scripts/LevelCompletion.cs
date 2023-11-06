using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletion : MonoBehaviour
{
    private PlayerRanking playerRanking;
    private Timer gameTimer;
    private void Start()
    {
        gameTimer = GetComponent<Timer>();
        playerRanking = GetComponent<PlayerRanking>();
    }

    public void OnLevelComplete()
    {
        gameTimer.StopTimer();
        playerRanking.CalculateRank();
    }
}