using UnityEngine;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour
{
    public Image maskOverlay;
    private bool isChefInTrigger = false;
    // Reference to the PlayerMovementDevelopment script
    public PlayerMovementDevelopment playerMovementDevelopment;

    void Update()
    {
        if (isChefInTrigger && (playerMovementDevelopment.currentPlayerState == PlayerPowerState.ELECTRIC_ACTIVE) && (Input.GetKeyDown(KeyCode.E)))
        {
            TriggerButtonFunctionality();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChefInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChefInTrigger = false;
        }
    }

    void TriggerButtonFunctionality()
    {
        // Trigger the button's functionality, aka. toggle the dark overlay
        // Set the alpha of the dark overlay to 0 to make it fully transparent
        Color currentColor = maskOverlay.color;
        currentColor.a = 0;
        maskOverlay.color = currentColor;

    }
}
