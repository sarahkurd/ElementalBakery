using System.Collections;
using models;
using UnityEngine;

public enum IngredientCookingState
{
    UNCOOKED, COOKING, COMPLETE, BURNED, DISPOSED
}

public class IngredientController : MonoBehaviour
{
    // Components
    private SpriteRenderer spriteRenderer;
    private ProgressBar progressBarScript;
    
    // Manage state variables
    public IngredientCookingState currentIngredientState = IngredientCookingState.UNCOOKED;
    private Ingredient ingredient;
    
    // Manage Children of this empty game object
    private GameObject ingredientGameObject;
    private GameObject progressBarUiCanvas;

    private bool isIngredientBurned;
    
    // Start is called before the first frame update
    void Start()
    {
        // get children game objects nested under this empty game object 
        ingredientGameObject = this.transform.GetChild(0).gameObject;
        progressBarUiCanvas = this.transform.GetChild(1).gameObject;
        
        // will be used to update color of sprite if it is burned
        spriteRenderer = ingredientGameObject.GetComponent<SpriteRenderer>();
        
        // create an instance of an Ingredient data object
        ingredient = IngredientMap.dict[ingredientGameObject.name];
        Debug.Log(ingredient.name);
        
        // set the timer in the progress bar
        progressBarScript = progressBarUiCanvas.transform.GetChild(0).gameObject.GetComponent<ProgressBar>();
        progressBarScript.SetTimer(ingredient.timeToCook);
    }

    // Update is called once per frame
    void Update()
    {
        if (progressBarUiCanvas.activeSelf)
        {
            if (isIngredientBurned)
            {
                Debug.Log("Ingredient burned");
                currentIngredientState = IngredientCookingState.BURNED;
                DestroyIngredientAndProgressBar();
            }
            else if (currentIngredientState != IngredientCookingState.COMPLETE && progressBarScript.IsComplete())
            {
                Debug.Log("Timer complete");
                currentIngredientState = IngredientCookingState.COMPLETE;
                // start timer that will give some wiggle room before it burns
                StartCoroutine(StartIngredientCompleteTimer());
            } 
            else if (currentIngredientState != IngredientCookingState.COMPLETE)
            {
                currentIngredientState = IngredientCookingState.COOKING;
            }
        }
    }

    public void EnableProgressBar()
    {
        progressBarUiCanvas.SetActive(true);
    }
    
    public void DisableProgressBar()
    {
        progressBarUiCanvas.SetActive(false);
    }

    private IEnumerator StartIngredientCompleteTimer()
    {
        yield return new WaitForSeconds(2.0f); 
        spriteRenderer.color = new Color(73.0f, 30.0f, 30.0f);
        yield return new WaitForSeconds(3.0f);
        isIngredientBurned = true;
    }

    public void DestroyIngredientAndProgressBar()
    {
        Destroy(this.gameObject);
    }
}
