using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 
using TMPro;
using UnityEngine.SceneManagement;

public class GetPlayerInput : MonoBehaviour
{   
    public TMP_InputField playerNameInput;
    public Button startGameButton; 
    public string targetSceneName;
    // Start is called before the first frame update
    void Start()
    {   startGameButton.interactable = false;

        // Add an event listener to the input field to check for changes.
        playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);

        // Add a click event listener to the "StartGame" button.
        startGameButton.onClick.AddListener(OnStartGameClick);
        
    }

    private void OnPlayerNameChanged(string newName)
    {
        // Check if the player's name is not empty and enable the button accordingly.
        startGameButton.interactable = !string.IsNullOrEmpty(newName);
    }
    // Update is called once per frame
    private void OnStartGameClick()
    {
        // Load the target scene when the "StartGame" button is clicked.
        SceneManager.LoadScene(targetSceneName);
    }
    void Update()

    {   
    } 
        
}

