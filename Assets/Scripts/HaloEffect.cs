using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloEffect : MonoBehaviour
{    private SpriteRenderer haloSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        SetOpacity(0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Enable the gameObject
            Debug.Log("Player Entered!"); 
             SetOpacity(1f);
        }
    }

     private void OnTriggerExit2D (Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {   Debug.Log("Player exited!"); 
            // Disable the gameObject
           SetOpacity(0f);
        }
    }


     private void SetOpacity(float alpha)
    {
        // Make sure the SpriteRenderer is initialized
        if (haloSpriteRenderer == null)
        {
            haloSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Change the color with the desired alpha value
        Color spriteColor = haloSpriteRenderer.color;
        spriteColor.a = alpha;
        haloSpriteRenderer.color = spriteColor;
    }
}
