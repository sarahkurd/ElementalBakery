
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("level0");
    }
    //shut down the game when the quit button is clicked
    public void QuitGame()
    {
        Application.Quit();
    }
}
