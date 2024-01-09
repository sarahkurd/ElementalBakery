using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarManager : MonoBehaviour
{   
    public float colorTransitionSpeed = 0.2f; // Speed of color transition
    public float delayBetweenElements = 0.3f; // Delay between transitioning each element
    public float disappearanceDelay = 0.75f;
    private float cookTime;
    public bool isComplete = false;
    private int currentIndex = 0;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(TransitionColors()); 
    }


    private IEnumerator TransitionColors(){
        //Debug.Log("Ingredient Cooktime: " + this.cookTime); 
        int childCount = transform.childCount; 
        for(int i=0; i< childCount; i++){
            
            //child.GetComponent<SpriteRenderer>().color 
            if (i > 0)
                yield return new WaitForSeconds(delayBetweenElements);
            
            GameObject image = this.transform.GetChild(i).gameObject; 
            Color targetColor = assignedColors[i];
            Color startColor = image.GetComponent<SpriteRenderer>().color;
            
            float startTime = Time.time;
            float colorTransitionTime = colorTransitionSpeed;

            while (Time.time < startTime + colorTransitionSpeed)
            {
                float journeyCovered = (Time.time - startTime) / colorTransitionTime;
                //float fractionOfJourney = journeyCovered / journeyLength;
                image.GetComponent<SpriteRenderer>().color= Color.Lerp(startColor, targetColor, journeyCovered);
                yield return null; 
            }

            currentIndex = i; 
            if (i == childCount - 1)
            {
               
                yield return new WaitForSeconds(disappearanceDelay);
                isComplete = true;
                
                //StartCoroutine(ColorBlinking()); 
              
            }


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
