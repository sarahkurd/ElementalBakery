using System.Collections;
using System.Collections.Generic;
using models;
using UnityEngine;

enum IngredientCookingState
{
    UNCOOKED, COOKING, COMPLETE, BURNED, DISPOSED
}

public class IngredientController : MonoBehaviour
{
    public GameObject ingredientGameObject;
    
    // Components
    private SpriteRenderer spriteRenderer;
    
    // Manage state variables
    private IngredientCookingState currentIngredientState = IngredientCookingState.UNCOOKED;
    private Ingredient ingredient;
    
    // Start is called before the first frame update
    void Start()
    {
        // will be used to update color of sprite if it is burned
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // create an instance of an Ingredient data object
        ingredient = IngredientMap.dict[ingredientGameObject.name];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
