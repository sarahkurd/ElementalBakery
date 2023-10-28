using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] private PlayerRanking playerRanking;
    [SerializeField] private Timer gameTimer;

    private void Start()
    {
        if (!gameTimer) gameTimer = GetComponent<Timer>();
        if (!playerRanking) playerRanking = GetComponent<PlayerRanking>();
    }

    public void OnLevelComplete()
    {
        gameTimer.StopTimer();
        playerRanking.CalculateRank();
    }
}