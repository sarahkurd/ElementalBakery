using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateHalo : MonoBehaviour
{   public GameObject halo; 
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ActivateHaloEffect();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            DeactivateHaloEffect();
        }
    }

     void ActivateHaloEffect()
    {
        // Activate the halo
        halo.SetActive(true);

        // Position the halo behind the 2D game object
        //halo.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
    }

    void DeactivateHaloEffect()
    {
        // Deactivate the halo
        halo.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
