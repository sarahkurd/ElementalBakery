
using UnityEngine;
using TMPro;

public class TutorialTextManager : MonoBehaviour
{
    public static TutorialTextManager Instance { get; private set; }

    public TextMeshProUGUI tutorialText; // Assign this via the inspector

    private void Awake()
    {
        // Singleton pattern to ensure only one instance is active.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisplayMessage(string message)
    {
        tutorialText.text = message;
    }

    public void ClearMessage()
    {
        tutorialText.text = "";
    }
}
