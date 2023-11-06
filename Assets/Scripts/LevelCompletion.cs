using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletion : MonoBehaviour
{
    private LevelManager levelManager;
    private Timer gameTimer;
    private PlayerRanking playerRanking;

    // Add a field for the next scene's name or index
    [SerializeField] private string nextSceneName;

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
            OnLevelComplete();
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

    private IEnumerator WaitAndLoadNextScene()
    {
        Debug.Log("Entered coroutine, waiting for 3 seconds.");
        yield return new WaitForSeconds(3f);
        Debug.Log("3 seconds passed, now loading next scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

}
