using System;
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
    private float cookTime;
    public bool isComplete = false;
    private int currentIndex = 0;
    public Transform targetSprite; 
    private RectTransform rectTransform; 
    private bool disabled = true;
    private Color[] assignedColors = {
        new Color(1.0f, 0.0f, 0.0f),
        new Color(1.0f, 0.5f, 0.0f),
        new Color(1.0f, 1.0f, 0.0f),
         new Color(0.0f, 1.0f, 0.0f)
    }; 

    private Color[] transitionColors = {
        new Color(1.0f, 0.0f, 0.0f),//red 
        new Color(1.0f, 0.0f, 0.0f),//red 
        new Color(1.0f, 0.0f, 0.0f), //red 
        new Color(1.0f, 0.0f, 0.0f),//red 
        new Color(1.0f, 0.5f, 0.0f), //orange
        new Color(1.0f, 0.5f, 0.0f), //orange
        new Color(1.0f, 1.0f, 0.0f), //yellow
        new Color(1.0f, 1.0f, 0.0f) //yellow
    }; 
    

    private Color defaultColor = new Color(250.0f, 0f, 0f); 
    private Color cookedColor = new Color(0.0f, 1.0f, 0.0f); 
    public Vector3 offset = new Vector3(0, 50, 0);
    // Start is called before the first frame update
    void Start()
    {   rectTransform = GetComponent<RectTransform>(); 
        
        
    }

    void OnEnable()
    {
        disabled = false;
        StartCoroutine(TransitionColors());
    }

    private void OnDisable()
    {
        disabled = true;
    }

    // Update is called once per frame
    void Update()
    {   if(targetSprite!=null){
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetSprite.position);
            screenPos += offset;

            // Set the UI element's position to the screen position
            rectTransform.position = screenPos;
        }
        
    }

    private IEnumerator TransitionColors()
    {   
        Debug.Log("Ingredient Cooktime: " + this.cookTime); 
        
        for (int i = 0; i < imageElements.Length; i++)
        {
            while (disabled) { }
            if (i > 0)
                yield return new WaitForSeconds(delayBetweenElements);

            Image image = imageElements[i];
            Color targetColor = assignedColors[i];
            
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
               
                yield return new WaitForSeconds(disappearanceDelay);
                isComplete = true;
                
                StartCoroutine(ColorBlinking()); 
              
            }
        }
    }

    private IEnumerator ColorBlinking(){
        int counter = transitionColors.Length - 1;
        while (disabled) { }
       
        while(true){
            // turn all points to green 
            for (int i = 0; i < imageElements.Length; i++)
            {
                imageElements[i].color =  transitionColors[counter]; 
            }
            yield return new WaitForSeconds(0.5f); 

            
            //blink transition from white to black 
            
                
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
            counter = transitionColors.Length - 1;
        }

        }
    }

    private void ChangeToDefaultColor(){

        for (int i = 0; i < imageElements.Length; i++)
        {
            imageElements[i].color = cookedColor; 

        }

    }

    public void SetTimer(float cookTime)
    {
        this.cookTime = cookTime;
        this.delayBetweenElements = cookTime / 4.0f;
       
    }

    public bool IsComplete()
    {
        return isComplete;
    }

}
