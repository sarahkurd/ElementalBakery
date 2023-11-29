using UnityEngine;
using UnityEngine.UI; // Needed for UI elements like Image

public class ShowImageOnTrigger : MonoBehaviour
{
    [SerializeField] private Image customImage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            customImage.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            customImage.enabled = false;
        }
    }
}
