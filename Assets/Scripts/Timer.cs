using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timeUpScreen;

   
    [SerializeField] private bool countdownTimer = false; // Determine if timer is countdown or stopwatch

    public float initialTime = 90f;
    private float elapsedTime;
    private float timeUsed;
    private bool timerActive = true;

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

        elapsedTime = countdownTimer ? initialTime : 0f;
    }

    void Update()
    {   
        //timeUpScreen.SetActive(true);
        if (!timerActive || timerText == null) return;
        // Add a Debug statement to log every frame
        // Debug.Log("Timer Update: elapsedTime = " + elapsedTime + ", timeUsed = " + timeUsed);

        if (countdownTimer)
        {
            if (elapsedTime <= 0)
            {
                if (timerText != null) // Ensure there is a reference to update the text
                {
                    timerText.text = "Time Up!";
                }

                if (timeUpScreen != null) // Ensure there is a reference before calling SetActive
                {   //timerText.SetActive(false); 
                    timerText.text = "Time Up!";
                    timeUpScreen.SetActive(true);
                    Time.timeScale = 0;
                }

                timerActive = false;
                // When time runs out, time used should be equal to the initial time.
                timeUsed = initialTime;
            }
            else
            {
                elapsedTime -= Time.deltaTime;
                // Update time used as the countdown proceeds
                timeUsed = initialTime - elapsedTime;
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;
            // In a stopwatch scenario, time used is just the elapsed time
            timeUsed = elapsedTime;
        }

        
        // Ensure the timer display is updated only when timerText is not null.
        if (timerText != null)
        {
            UpdateTimerDisplay(elapsedTime);
        }
    }

    private void UpdateTimerDisplay(float time)
    {
        // The time to display is different based on whether it's a countdown or stopwatch.
        float displayTime = countdownTimer ? time : timeUsed;
        int minutes = Mathf.FloorToInt(displayTime / 60);
        int seconds = Mathf.FloorToInt(displayTime % 60);
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
