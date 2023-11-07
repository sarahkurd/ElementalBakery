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
    private float masterTimeThreshold = 60.0f; 
    private float greatChefTimeThreshold = 90.0f; 
    private float noviceChefTimeThreshold = 150.0f;
    private float unrankedChefTimeThreshold = 180.0f;

    private const int MaxScore = 100;
    private int playerScore = MaxScore; // reduce this score as they make mistakes or if they take too long
    //can be assigned anything between "master_chef", "great_chef", "novice_chef", "unranked" 

    public PlayerRank playerRank;
    private PlayerRanking PlayerRankingController;
    
    // Start is called before the first frame update
    void Start()
    {
        analyticsScript = analyticsManager.GetComponent<CollectAnalytics>();
        customerOrderGO = GameObject.FindWithTag("Customer").transform.GetChild(1).gameObject;
        startLevelTimer = Time.time; 
        playerRank = PlayerRank.Unranked;
        firstIngredientTimeCalculated = false;
        PlayerRankingController = GameObject.FindWithTag("Player").GetComponent<PlayerRanking>();
        PopulateCustomerOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLevelComplete){
            if (tookToLong)
            {
                PlayerRankingController.SetYouTookToLong();
            }
            else
            {
                UpdateLevelTimer(); 
                CalculatePlayerRank(); 
                CallAnalyticsManager();
            }
            isLevelComplete = false;
            tookToLong = false;
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
                    DecreasePlayerScore(11);
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
                DecreasePlayerScore(20);
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
            DecreasePlayerScore(25);
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
            playerRank = PlayerRank.Master;
        }
        else if(timeToFinishLevel > masterTimeThreshold && timeToFinishLevel <= greatChefTimeThreshold){
            DecreasePlayerScore(10);
        }
        else if (timeToFinishLevel > greatChefTimeThreshold && timeToFinishLevel <= noviceChefTimeThreshold){
            DecreasePlayerScore(15);
        } else if (timeToFinishLevel > noviceChefTimeThreshold && timeToFinishLevel <= unrankedChefTimeThreshold)
        {
            DecreasePlayerScore(20);
        }
        else
        {
            tookToLong = true;
        }

        if (!tookToLong)
        {
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
        }
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
    
    public bool IsLevelComplete { get { return isLevelComplete; } }
}
