using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timeUpScreen;
    private GameObject levelManager;
   
    [SerializeField] private bool countdownTimer = false; // Determine if timer is countdown or stopwatch


    private float elapsedTime;
    private float timeUsed;
    private bool timerActive = true;

    private GameObject background; 

    public float blinkSpeed = 1.0f; // Adjust this value to control the speed of blinking
    public Color blinkColor;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private float initialTime;
    private int levelNumber; 

    void Start()
    {   levelManager = GameObject.FindWithTag("LevelManager"); 
        LevelManager levelManagerScript = levelManager.GetComponent<LevelManager>();
        background = GameObject.FindWithTag("background");

        levelNumber = levelManagerScript.levelNumber;

        if(levelManagerScript.levelNumber< 0){
        // Implies these are tutorial levels. 
            initialTime = 300f; 
        }
        else if(levelManagerScript.levelNumber == 0){
            initialTime = 150f; 
        }
        else if(levelManagerScript.levelNumber == 1){
            initialTime = 210f;  //150f
        }
        else if(levelManagerScript.levelNumber == 2){
            initialTime = 240f; 

        }
        else if(levelManagerScript.levelNumber== 3){
            initialTime = 270f; 
        }
        else if(levelManagerScript.levelNumber== 4){
            initialTime = 360f; 
        }
        else if(levelManagerScript.levelNumber== 5){
            initialTime = 360f; 
        }

       
       
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

        if (countdownTimer &&  levelNumber > 0)
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
        

        
        // Ensure the timer display is updated only when timerText is not null.
        if (timerText != null && levelNumber > 0)
        {
            UpdateTimerDisplay(elapsedTime);
        }


        /*
        if (countdownTimer && elapsedTime <=20f){
            StartCoroutine(BlinkingBackground());
        }
        */ 

        //if (countdownTimer && elapsedTime <=20f){
        //    StartCoroutine(BlinkingBackground());
        //}



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


    private IEnumerator BlinkingBackground()
    {
        // Get the SpriteRenderer component of the background GameObject
        spriteRenderer = background.GetComponent<SpriteRenderer>();

        // Save the original color of the background
        originalColor = spriteRenderer.color;

        while (elapsedTime <= 20f)
        {
            // Blink to red
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(1f / blinkSpeed);

            // Blink back to original color
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(1f / blinkSpeed);
        }

        // Ensure the background returns to its original color after blinking
        spriteRenderer.color = originalColor;
    }
    
}
