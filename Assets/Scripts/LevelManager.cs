using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject analyticsManager; // pass in a reference to the AnalyticsManager game object

    private CollectAnalytics analyticsScript;
    private GameObject customerOrderGO;
    private Dictionary<string, int> customerOrder = new Dictionary<string, int>();
    private Dictionary<string, IngredientCookingState> playerCollected = new Dictionary<string, IngredientCookingState>();
    private int incorrectOrderCount;
    private int incorrectIngredientCollectedCount;
    private int incorrectIngredientStateCount;
    private bool isLevelComplete;

    private float startLevelTimer; 
    private float timeToFinishLevel; 
    public int levelNumber;     

    private float timeToGetFirstIngredient; 
    private bool firstIngredientTimeCalculated; 
    //variables for ranking player performance 
    private float rankSThreshold = 30.0f; 
    private float rankAThreshold = 50.0f; 
    private float rankBThreshold = 80.0f;  
    //can be assigned anything between "master_chef", "great_chef", "novice_chef", "unranked" 

    private string playerRank; 

    // Start is called before the first frame update
    void Start()
    {
        analyticsScript = analyticsManager.GetComponent<CollectAnalytics>();
        customerOrderGO = GameObject.FindWithTag("Customer").transform.GetChild(1).gameObject;
        startLevelTimer = Time.time; 
        playerRank = "unranked";
        firstIngredientTimeCalculated = false;  
        PopulateCustomerOrder();
    }

    // Update is called once per frame
    void Update()
    {   
        if(isLevelComplete){ 
            UpdateLevelTimer(); 
            CalculatePlayerRank(); 
            CallAnalyticsManager();

        }
        if(playerCollected.Count == 1 && !firstIngredientTimeCalculated){ 
            UpdateFirstIngredientCollection(); 
            firstIngredientTimeCalculated = true; 

        }
    }

    // should be called when player puts an ingredient on the plate
    public void PlateIngredient(string ingredientName, IngredientCookingState state)
    {
        playerCollected.Add(ingredientName, state);
        Debug.Log(playerCollected); 
    }

    // Should be called when player collides with the customer
    public bool CheckIfLevelComplete()
    {
        if (playerCollected.Count == 0)
        {
            // player has no items on the plate 
            // show a message on the screen
            Debug.Log("Player collected count is 0");
            return false;
        }
        
        // loop through player items. Check if customer order contains it.
        foreach (var item in playerCollected)
        {
            if (customerOrder.ContainsKey(item.Key)) // player collected item in the order
            {
                Debug.Log("customer order contains " + item.Key);
                if (item.Value != IngredientCookingState.COMPLETE)
                {
                    Debug.Log("incorrect state: " + item.Value);
                    incorrectIngredientStateCount++;
                }
                // whether the item is cooked/burned/uncooked, we still count it but if the
                // item state is not complete we keep a count (shown above) and deduce from their ranking
                int count = customerOrder[item.Key];
                if (count == 1)
                {
                    Debug.Log("remove  " + item.Key + " from customer order");
                    customerOrder.Remove(item.Key); 

                    //Need to add disabling the ingredient item that was collected. 
                }
                else
                {
                    customerOrder[item.Key] = count - 1;
                }
            }
            else
            {
                Debug.Log("incorrect ingredient collected");
                incorrectIngredientCollectedCount++;
            }
        }

        // check if all items collected
        if (customerOrder.Count == 0)
        {
            Debug.Log("isLevelComplete = true");

            isLevelComplete = true;
            return true;
        }
        else
        {
            Debug.Log("isLevelComplete = false");
            isLevelComplete = false;
            incorrectOrderCount++;
            PopulateCustomerOrder();
            return false;
        }
    }

    private void PopulateCustomerOrder()
    {
        customerOrder.Clear();
        foreach (Transform trans in customerOrderGO.transform)
        {
            string ingredientName = trans.gameObject.name;
            if (customerOrder.ContainsKey(ingredientName))
            {
                int currentCount = customerOrder[ingredientName];
                customerOrder[ingredientName] = currentCount + 1;
                Debug.Log(trans.gameObject.name + " count: " + (currentCount + 1));
            }
            else
            {
                customerOrder.Add(trans.gameObject.name, 1);
                Debug.Log(trans.gameObject.name);
            }
        }
    }

    private void UpdateLevelTimer(){

        timeToFinishLevel = Time.time - startLevelTimer; 


    }
    private void UpdateFirstIngredientCollection(){
        timeToGetFirstIngredient = Time.time - startLevelTimer; 

    }

    private void CalculatePlayerRank(){
        //One player per level 
        if(timeToFinishLevel <= rankSThreshold){
            playerRank = "master_chef"; 
        }
        else if(timeToFinishLevel> rankSThreshold && timeToFinishLevel <= rankAThreshold){
            playerRank = "great_chef"; 
        }
        else{
            playerRank = "novice_chef"; 
        }
        Debug.Log("rank of the player"+ playerRank); 
    }

    private void CallAnalyticsManager(){ 
        CollectAnalytics analyticsScript = analyticsManager.GetComponent<CollectAnalytics>(); 
        
        analyticsScript.putAnalytics(timeToFinishLevel, timeToGetFirstIngredient); 
    }
 
}
