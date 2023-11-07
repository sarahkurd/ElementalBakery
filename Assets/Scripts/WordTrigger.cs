using UnityEngine;

public class WordTrigger : MonoBehaviour
{
    public string messageToShow; // The message to show when the player enters the trigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            Debug.Log("Trigger Entered: " + messageToShow);
            TutorialTextManager.Instance.DisplayMessage(messageToShow);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger Exited");
            TutorialTextManager.Instance.ClearMessage();
        }
    }
}
