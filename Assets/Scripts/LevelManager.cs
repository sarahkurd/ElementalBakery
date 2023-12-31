using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerRank
{
    Master, Great, Novice, Unranked
}

public class LevelManager : MonoBehaviour
{
    public GameObject analyticsManager; // pass in a reference to the AnalyticsManager game object
    private FallCountCatch countFallenScript;  
    public GameObject fallCounter; 
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
    public bool tookToLong = false;
    
    //variables for ranking player performance 
    private float masterTimeThreshold = 80.0f; 
    private float greatChefTimeThreshold = 120.0f; 
    private float noviceChefTimeThreshold = 200.0f;
    private float unrankedChefTimeThreshold = 240.0f;

    private const int MaxScore = 100;
    private int playerScore = MaxScore; // reduce this score as they make mistakes or if they take too long
    //can be assigned anything between "master_chef", "great_chef", "novice_chef", "unranked" 

    public PlayerRank playerRank;
    private PlayerRanking PlayerRankingController;
    private Vector3 playerStartPosition; 
    
    // Start is called before the first frame update
    void Start()
    {
        analyticsScript = analyticsManager.GetComponent<CollectAnalytics>();
        countFallenScript = fallCounter.GetComponent<FallCountCatch>(); 


        customerOrderGO = GameObject.FindWithTag("Customer").transform.GetChild(1).gameObject;
        startLevelTimer = Time.time; 
        playerRank = PlayerRank.Unranked;
        firstIngredientTimeCalculated = false;
        playerStartPosition = GameObject.FindWithTag("Player").transform.position; 
        PlayerRankingController = GameObject.FindWithTag("Player").GetComponent<PlayerRanking>();

        PopulateCustomerOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLevelComplete){
            UpdateLevelTimer(); 
            CalculatePlayerRank(); 
            CallAnalyticsManager();
            isLevelComplete = false;
        }
        
        if (playerCollected.Count == 1 && !firstIngredientTimeCalculated){ 
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
                    DecreasePlayerScore(25);
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
                DecreasePlayerScore(50);
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
            DecreasePlayerScore(35);
            PopulateCustomerOrder();
            PlayerRankingController.DisplayIncorrectOrder();
            return false;
        }
    }

    private void DecreasePlayerScore(int decrease)
    {
        if (playerScore != 0)
        {
            playerScore -= decrease;
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
        PlayerRankingController.SetTotalTime(timeToFinishLevel);
    }
    
    private void UpdateFirstIngredientCollection(){
        timeToGetFirstIngredient = Time.time - startLevelTimer;
    }

    private void CalculatePlayerRank(){
        //One player per level 
        if(timeToFinishLevel <= masterTimeThreshold){
            Debug.Log("less than Master threshold");
            DecreasePlayerScore(0);
        }
        else if(timeToFinishLevel > masterTimeThreshold && timeToFinishLevel <= greatChefTimeThreshold){
            Debug.Log("less than great threshold");
            DecreasePlayerScore(20);
        }
        else if (timeToFinishLevel > greatChefTimeThreshold && timeToFinishLevel <= noviceChefTimeThreshold){
            Debug.Log("less than novice threshold");
            DecreasePlayerScore(25);
        } else if (timeToFinishLevel > noviceChefTimeThreshold && timeToFinishLevel <= unrankedChefTimeThreshold)
        {
            Debug.Log("less than unranked threshold");
            DecreasePlayerScore(30);
        }
        
        Debug.Log("Player Score: " + playerScore);
        if (playerScore >= 80)
        {
            playerRank = PlayerRank.Master;
        }
        else if (playerScore >= 60)
        {
            playerRank = PlayerRank.Great;
        }
        else if (playerScore >= 40)
        {
            playerRank = PlayerRank.Novice;
        }
        else
        {
            playerRank = PlayerRank.Unranked;
        }
        
        PlayerRankingController.SetRankingText(playerRank);
        PlayerRankingController.SetHeaderText(playerRank);
        PlayerRankingController.SetIncorrectIngredientsText(incorrectIngredientStateCount);
        PlayerRankingController.SetIncorrectPlatesDelivered(incorrectOrderCount);
        Debug.Log("rank of the player: " + playerRank.ToString()); 
    }

    private void CallAnalyticsManager(){ 
        CollectAnalytics analyticsScript = analyticsManager.GetComponent<CollectAnalytics>(); 
        
        analyticsScript.putAnalytics(
            levelNumber, 
            timeToFinishLevel, 
            timeToGetFirstIngredient, 
            incorrectIngredientCollectedCount, 
            incorrectIngredientStateCount,
            playerRank.ToString() 
            ); 
    }
    
    
    private void OnCollisionEnter2D(Collision2D other)
    {   Debug.Log("Player fell!");
        if(other.gameObject.CompareTag("Player")){
            other.transform.position = playerStartPosition; 
        }
        
    }
    
    public bool IsLevelComplete { get { return isLevelComplete; } }
}
