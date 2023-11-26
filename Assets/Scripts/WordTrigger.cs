
using UnityEngine;
using UnityEngine.UI;

public class WordTrigger : MonoBehaviour
{
    public Image imageToShow; // Assign this in the Inspector

    private void Start()
    {
        // Ensure the image is hidden on scene start
        if (imageToShow != null)
        {
            imageToShow.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Image to show not assigned in WordTrigger script.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            if (imageToShow != null)
            {
                imageToShow.gameObject.SetActive(true); // Show the image
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (imageToShow != null)
            {
                imageToShow.gameObject.SetActive(false); // Hide the image when the player leaves the trigger
            }
        }
    }
}



