using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveMechanism : MonoBehaviour
{   public GameObject stoveObject; 
    public GameObject ingredientList; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   if(transform.childCount == 1)
        {    
           
            Transform ingredientChildTransform = transform.GetChild(0); 
            GameObject ingredientChild = ingredientChildTransform.gameObject; 
            Debug.Log("child of ingredientChild"+ingredientChildTransform.GetChild(0).gameObject.name); 
            IngredientController ic = ingredientChild.GetComponent<IngredientController>();
            if(ic.currentIngredientState == IngredientCookingState.BURNED){
                
               ingredientChildTransform.GetChild(0).transform.GetComponent<Collider2D>().isTrigger = true;; 
              
               ingredientChildTransform.SetParent(ingredientList.transform); 
            }
        }

        
    }
}
