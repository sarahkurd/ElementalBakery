using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletion : MonoBehaviour
{
    private LevelManager levelManager;
    private Timer gameTimer;
    private PlayerRanking playerRanking;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playerRanking = FindObjectOfType<PlayerRanking>();
        gameTimer = GetComponent<Timer>();

        if (levelManager == null) Debug.LogError("LevelManager not found in the scene.");
        if (gameTimer == null) Debug.LogError("Timer component not found on the GameObject.");
    }

    private void Update()
    {
        if (levelManager.IsLevelComplete)
        {
            if (levelManager.tookToLong)
            {
                OnTookToLong();
            }
            else
            {
                OnLevelComplete();
            }
        }
    }

    public void OnLevelComplete()
    {
        Debug.Log("Level complete. Stopping timer and showing rank.");
        gameTimer.StopTimer();
        playerRanking.DisplayRank();
        Debug.Log("Starting coroutine to wait 3 seconds before loading the next scene.");
        StartCoroutine(WaitAndLoadNextScene());
    }

    public void OnTookToLong()
    {
        Debug.Log("Level complete. Took to long.");
        gameTimer.StopTimer();
        playerRanking.DisplayRank();
        StartCoroutine(WaitAndLoadNextScene());
    }

    private IEnumerator WaitAndLoadNextScene()
    {
        Debug.Log("Entered coroutine, waiting for 8 seconds.");
        yield return new WaitForSeconds(8f);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log("3 seconds passed, now loading next scene index: " + nextSceneIndex);
        SceneManager.LoadScene(nextSceneIndex);
    }
}