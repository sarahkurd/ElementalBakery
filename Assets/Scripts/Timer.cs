using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject timeUpScreen;
    [SerializeField] bool countdownTimer = false; // Determine if timer is countdown or stopwatch

    float elapsedTime, timeUsed;
    float initialTime = 90f; // Set this to the starting time of your timer
    bool timerActive = true;

    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("TimerText is not assigned!");
        }

        if (timeUpScreen == null)
        {
            Debug.LogError("TimeUpScreen is not assigned!");
        }

        if (countdownTimer)
            elapsedTime = initialTime;
        else
            elapsedTime = 0f;
    }

    void Update()
    {
        if (!timerActive || timerText == null) return;

        if (countdownTimer)
        {
            if (elapsedTime <= 0)
            {
                timerText.text = "Time Up!";
                if (timeUpScreen != null)
                {
                    timeUpScreen.SetActive(true);
                }
                timerActive = false;
                timeUsed = initialTime;
            }
            else
            {
                elapsedTime -= Time.deltaTime;
                timeUsed = initialTime - elapsedTime;
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;
            timeUsed = elapsedTime;
        }

        UpdateTimerDisplay(elapsedTime);
    }

    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public float ElapsedTime
    {
        get { return elapsedTime; }
    }

    public float TimeUsed
    {
        get { return timeUsed; }
    }
}
