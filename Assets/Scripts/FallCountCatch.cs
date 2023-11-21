using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCountCatch : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 playerStartPosition; 
    public GameObject playerStartPositionTracker; 
    public int countNumTimesFallen = 0; 
    void Start()
    {   playerStartPosition = playerStartPositionTracker.transform.position; 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D other)
    {   if(other.gameObject.CompareTag("Player")){
            
            other.transform.position = playerStartPosition; 
            StartCoroutine(FallCoroutine()); 
            countNumTimesFallen++; 

        }
        
    }

     private IEnumerator FallCoroutine()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
    } 
}
