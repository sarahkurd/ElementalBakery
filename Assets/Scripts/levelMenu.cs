using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class levelMenu : MonoBehaviour
{

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
