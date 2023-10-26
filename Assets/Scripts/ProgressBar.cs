using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{   public Image healthBar; 
    public Image[] imageElements; // Reference to the 10 Image UI elements
    public float colorTransitionSpeed = 0.2f; // Speed of color transition
    public float delayBetweenElements = 0.3f; // Delay between transitioning each element
    public float disappearanceDelay = 0.75f;
    private long cookTime;
    public bool isComplete = false;
    private int currentIndex = 0;
    private Color[] assignedColors = {
        new Color(1.0f, 0.0f, 0.0f),
        new Color(1.0f, 0.5f, 0.0f),
        new Color(1.0f, 1.0f, 0.0f),
         new Color(0.0f, 1.0f, 0.0f)
    }; 

    

    private Color defaultColor = new Color(250.0f, 0f, 0f); 
    private Color cookedColor = new Color(0.0f, 1.0f, 0.0f); 
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    void OnEnable(){
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

            //StartCoroutine(FadeInImage(image));
            //yield return new WaitForSeconds(fadeInSpeed);
            
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
                isComplete = true;
                //ChangeToDefaultColor(); 
                StartCoroutine(ColorBlinking()); 
                //StartCoroutine(DisappearImages());
                //DestroyElements();
            }
        }
    }

    private IEnumerator ColorBlinking(){
        int counter = assignedColors.Length - 1; 
        while(true){
            for (int i = 0; i < imageElements.Length; i++)
            {
                imageElements[i].color = assignedColors[counter]; 

            }
            yield return new WaitForSeconds(0.5f); 
            for (int i = 0; i < imageElements.Length; i++)
            {
                imageElements[i].color =  new Color(1.0f, 1.0f, 1.0f); 

            }
            yield return new WaitForSeconds(0.125f); 
            for (int i = 0; i < imageElements.Length; i++)
            {
                imageElements[i].color =  new Color(0.0f, 0.0f, 0.0f); 

            }
            yield return new WaitForSeconds(0.125f); 
        
        counter --; 
        if(counter <0){
            counter = assignedColors.Length - 1;
        }

        }
    
        
    }

    private void ChangeToDefaultColor(){

        for (int i = 0; i < imageElements.Length; i++)
        {
            imageElements[i].color = cookedColor; 

        }

    }

    public void SetTimer(long cookTime)
    {
        this.cookTime = cookTime;
        //delayBetweenElements = cookTime / 4.0f;
    }

    public bool IsComplete()
    {
        return isComplete;
    }

}
