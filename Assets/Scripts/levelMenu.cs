using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class levelMenu : MonoBehaviour
{
    //public Button[] buttons;

    //private void Awake()
    //{

    //    int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
    //    for (int i = 0; i < buttons.Length; i++)
    //    {
    //        buttons[i].interactable = false;

    //    }
    //    for (int i = 0; i < buttons.Length && i < unlockedLevel; i++)
    //    {
    //        buttons[i].interactable = true;
    //    }

    //}
    public void OpenLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    //public void OpenLevel(int levelId)
    //{
    //    string levelName = "Level " + levelId;
    //    SceneManager.LoadScene(levelName);
    //}


}
