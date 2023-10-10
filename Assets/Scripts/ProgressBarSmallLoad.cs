using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarSmallLoad : MonoBehaviour
{   public Image healthBar; 
    public Image[] imageElements; // Reference to the 10 Image UI elements
    public float colorTransitionSpeed = 0.25f; // Speed of color transition
    public float delayBetweenElements = 0.5f; // Delay between transitioning each element
    public float disappearanceDelay = 0.75f; // Delay before making the 10th element disappear
    public float fadeInSpeed = 0.2f;
    private int currentIndex = 0;

     private Color[] assignedColors = {
        new Color(1.0f, 0.0f, 0.0f), //Red
        new Color(1.0f, 0.5f, 0.0f), //Orange
        new Color(1.0f, 1.0f, 0.0f), //Yellow
        new Color(0.0f, 1.0f, 0.0f), //Green
     }; 
    // Start is called before the first frame update
    void Start()
    {
         StartCoroutine(TransitionColors());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private IEnumerator TransitionColors()
    {
        for (int i = 0; i < imageElements.Length; i++)
        {
            if (i > 0)
                yield return new WaitForSeconds(delayBetweenElements);

            Image image = imageElements[i];
            Color targetColor = assignedColors[i];

            StartCoroutine(FadeInImage(image));
            yield return new WaitForSeconds(fadeInSpeed);
            
            float startTime = Time.time;
            float journeyLength = Vector4.Distance(image.color, targetColor);

            while (Time.time < startTime + colorTransitionSpeed)
            {
                float journeyCovered = (Time.time - startTime) * colorTransitionSpeed;
                float fractionOfJourney = journeyCovered / journeyLength;
                image.color = Color.Lerp(image.color, targetColor, fractionOfJourney);
                yield return null;
            }

            currentIndex = i;

            if (i == imageElements.Length - 1)
            {
                // If this is the 10th element, start the disappearance process
                yield return new WaitForSeconds(disappearanceDelay);
                //StartCoroutine(DisappearImages());
                DestroyElements();
            }
        }
    }

    private void DestroyElements()
    {
        // Check if the last element exists
        if (currentIndex == imageElements.Length - 1)
        {    for (int i = 0; i < imageElements.Length; i++)
             { 
                Destroy(imageElements[i].gameObject);
             }
        }
        Destroy(healthBar); 
    }
   private IEnumerator DisappearImages()
    {
        yield return new WaitForSeconds(disappearanceDelay);

        for (int i = 0; i < imageElements.Length; i++)
        {
            if (i != currentIndex)
                imageElements[i].color = Color.black;
        }
    }

    private IEnumerator FadeInImage(Image image)
    {
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        float startTime = Time.time;
        float journeyLength = 1.0f; // The full opacity

        while (Time.time < startTime + fadeInSpeed)
        {
            float journeyCovered = (Time.time - startTime) * fadeInSpeed;
            float fractionOfJourney = journeyCovered / journeyLength;
            canvasGroup.alpha = Mathf.Lerp(0, 1, fractionOfJourney);
            yield return null;
        }
    }

}
