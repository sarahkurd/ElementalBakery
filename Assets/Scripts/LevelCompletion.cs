using UnityEngine;

public class LevelCompletion : MonoBehaviour
{
    private LevelManager levelManager;
    private Timer gameTimer;
    private PlayerRanking playerRanking;

    private void Start()
    {
        // Find the LevelManager instance in the scene.
        levelManager = FindObjectOfType<LevelManager>();
        playerRanking = FindObjectOfType<PlayerRanking>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene.");
            return;
        }

        // Assuming the Timer component is on the same GameObject as LevelCompletion.
        gameTimer = GetComponent<Timer>();
        if (gameTimer == null)
        {
            Debug.LogError("Timer component not found on the GameObject.");
        }
    }

    private void Update()
    {
        // Check if the level is complete by accessing the IsLevelComplete property.
        if (levelManager.IsLevelComplete)
        {
            OnLevelComplete();
        }
    }

    public void OnLevelComplete()
    {
        // Stop the timer when the level is complete.
        gameTimer.StopTimer();
        playerRanking.DisplayRank();
    }
}
