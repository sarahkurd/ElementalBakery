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
    
    // Start is called before the first frame update
    void Start()
    {
        analyticsScript = analyticsManager.GetComponent<CollectAnalytics>();
        customerOrderGO = GameObject.FindWithTag("Customer").transform.GetChild(1).gameObject;
        PopulateCustomerOrder();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // should be called when player puts an ingredient on the plate
    public void PlateIngredient(string ingredientName, IngredientCookingState state)
    {
        playerCollected.Add(ingredientName, state);
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
}
