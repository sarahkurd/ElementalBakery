using System.Collections;
using UnityEngine;
using TMPro;

public class HintOverlayController : MonoBehaviour
{
    public GameObject hintPanel; // Reference to the hint panel in the Unity Inspector
    public TMP_Text hintText; // Reference to the TextMeshPro Text component of the hint panel

    private bool hintShown = false; // Track whether the hint has been shown

    private void Start()
    {
        // Hide the hint panel at the start of the level
        hintPanel.SetActive(false);
    }

    private void Update()
    {
        // TODO: Change this function case by case
        // Check for arrow key presses
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Check if the hint has not been shown yet
            if (!hintShown)
            {
                // Show the hint and set the hint text
                ShowHint("Stepping onto the brown platform will cause it to break.");
                hintShown = true; // Mark the hint as shown
            }
        }
    }

    public void ShowHint(string hintMessage)
    {
        // Show the hint panel and set the hint text
        hintText.text = hintMessage;
        hintPanel.SetActive(true);

        // Start a coroutine to hide the hint after 3 seconds
        StartCoroutine(HideHintAfterDelay(3.0f));
    }

    private IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide the hint panel after the specified delay
        hintPanel.SetActive(false);
    }

    public void HideHint()
    {
        // Hide the hint panel
        hintPanel.SetActive(false);
    }
}