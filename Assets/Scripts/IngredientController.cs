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

    public bool isIngredientBurned;
    private Vector3 initialPosition; 
    
    // Start is called before the first frame update
    void Start()
    {
        // get children game objects nested under this empty game object 
        ingredientGameObject = this.transform.GetChild(0).gameObject;
        progressBarUiCanvas = this.transform.GetChild(1).gameObject;
        
        // will be used to update color of sprite if it is burned
        spriteRenderer = ingredientGameObject.GetComponent<SpriteRenderer>();
        initialPosition = ingredientGameObject.transform.position; 
        // create an instance of an Ingredient data object
        ingredient = IngredientMap.dict[ingredientGameObject.name];
        // set the timer in the progress bar
        progressBarScript = progressBarUiCanvas.transform.GetChild(0).gameObject.GetComponent<ProgressBar>();
        progressBarScript.SetTimer(ingredient.timeToCook);
        //Debug.Log(ingredient.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (progressBarUiCanvas.activeSelf)
        {
            if (progressBarScript.isBurned)
            {
                Debug.Log("Ingredient burned");
                currentIngredientState = IngredientCookingState.BURNED;
              
                DisableProgressBar();
                
                StartCoroutine(RespawnIngredient());
                //Start();
            }
            else if (currentIngredientState != IngredientCookingState.COMPLETE && progressBarScript.IsComplete())
            {
                Debug.Log("Timer complete");
                currentIngredientState = IngredientCookingState.COMPLETE;
                // start timer that will give some wiggle room before it burns
                StartCoroutine((progressBarScript.ColorBlinking()));
                
            } 
            else if (currentIngredientState != IngredientCookingState.COMPLETE)
            {
                currentIngredientState = IngredientCookingState.COOKING;
            }
        }
        SetIngredientStateColor(currentIngredientState); 
    }

    public void EnableProgressBar()
    {
        //Debug.Log("Enable progress bar");
        progressBarUiCanvas.SetActive(true);
    }
    
    public void DisableProgressBar()
    {
        //Debug.Log("Disable progress bar");
        progressBarUiCanvas.SetActive(false);
    }

    private IEnumerator StartIngredientCompleteTimer()
    {
        //yield return new WaitForSeconds(2.0f); 
        yield return new WaitForSeconds(5.0f);
        isIngredientBurned = true;
    }

    public void DestroyIngredientAndProgressBar()
    {
        Destroy(this.gameObject);
    }
    public IEnumerator RespawnIngredient()
    {   progressBarScript.isBurned=false; 
        progressBarScript.isComplete = false; 
        yield return new WaitForSeconds(2.0f);
        ingredientGameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);

        // Reset the position to the initial position
        ingredientGameObject.transform.position = initialPosition;
        isIngredientBurned = false;
        currentIngredientState = IngredientCookingState.UNCOOKED;

        // Restore the initial state of progressBarUiCanvas
        //progressBarUiCanvas.SetActive(initialProgressBarState);

        // Re-enable the object if needed
        ingredientGameObject.SetActive(true);
    }
    public void SetIngredientSprite(Sprite ingredientSprite)
    {
        spriteRenderer.sprite = ingredientSprite;
    }

    public void SetIngredientStateColor(IngredientCookingState state){
        if (state == IngredientCookingState.UNCOOKED){
             ingredientGameObject.GetComponent<SpriteRenderer>().color = new Color(0.55f, 0.55f, 0.55f);
        }
        else if (state == IngredientCookingState.COOKING){
             ingredientGameObject.GetComponent<SpriteRenderer>().color = new Color(0.79f, 0.79f, 0.79f);
        }
        else if (state == IngredientCookingState.COMPLETE){
            ingredientGameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        }
        else if (state == IngredientCookingState.BURNED) {
              ingredientGameObject.GetComponent<SpriteRenderer>().color = new Color(0.41f, 0.15f, 0.15f);
        }
        else if(state == IngredientCookingState.DISPOSED){ 
                 ingredientGameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        }
    }

    public bool CanApplyPower(PlayerPowerState state)
    {
        return (ingredient.cookType == CookType.WATER && state == PlayerPowerState.WATER_ACTIVE)
               || (ingredient.cookType == CookType.FIRE && state == PlayerPowerState.FIRE_ACTIVE)
               || (ingredient.cookType == CookType.AIR && state == PlayerPowerState.AIR_ACTIVE);
    }

    public CookType GetIngredientCookType()
    {
        return ingredient.cookType;
    }

    public void AnimateIngredientDrop()
    {
        // when player drops an ingredient, give some force in the x and y direction to look
        // like it is being tossed
    }
}
