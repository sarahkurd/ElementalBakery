using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimesUp : MonoBehaviour
{  
    public GameObject timesUpScreen;
    public GameObject timerPanel; 
    // Start is called before the first frame update
  

    void OnEnable(){
        timerPanel.SetActive(false); 
    }

    void OnDisable(){
        timerPanel.SetActive(true); 
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
 
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}



