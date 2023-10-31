using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking; 
using UnityEngine.SceneManagement; 
using System; 


public class CollectAnalytics : MonoBehaviour
{
    // Start is called before the first frame update
    private string sessionId; 
    public float timeToCompleteLevel; 
    public float timeToGetFirstIngredient; 
    public int numOfRetries; 
    public int numOfIngredients; 
    private bool isLevelCompleted = false; 
    private string URL = "https://docs.google.com/forms/d/e/1FAIpQLSf9EAqsHZbHqOVv3r5Csf8ft-4gARinCzh3zJqAxoUULG_5bQ/formResponse" ; 
    private void Awake(){
        
    }
    void Start()
    {   

    }

    // Update is called once per frame
    void Update()
    {   //Indicates a change in the level 
        
            //OnLevelCompletion(); 
    }
        
    
    public void putAnalytics(
                int levelNumber, 
                float finishTime, 
                float timeToGetFirstIngredient, 
                int incorrectIngredientCollectedCount, 
                int incorrectIngredientStateCount,
                string playerRank
                ){
       string sessionID; 
       long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        
       int randomValue = UnityEngine.Random.Range(0, 100000);
        
        
       sessionID = timestamp.ToString() + randomValue.ToString();
       StartCoroutine(Post(
            sessionID, 
            levelNumber.ToString(), 
            finishTime.ToString(), 
            timeToGetFirstIngredient.ToString(), 
            incorrectIngredientCollectedCount.ToString(), 
            incorrectIngredientStateCount.ToString(), 
            playerRank 
        )); 


    }

    private IEnumerator Post(
        string sessionId,  
        string levelNumber, 
        string finishTime, 
        string timeToGetFirstIngredient, 
        string incorrectIngredientCollectedCount, 
        string incorrectIngredientStateCount, 
        string playerRank 
        ){
        WWWForm form = new WWWForm(); 

        form.AddField("entry.337704600", sessionId ); 
        form.AddField("entry.578045944", levelNumber); 
        form.AddField("entry.666891163", finishTime); 
        form.AddField("entry.1115350779", timeToGetFirstIngredient); 
        form.AddField("entry.1128722234", incorrectIngredientCollectedCount); 
        form.AddField("entry.1053662980", incorrectIngredientStateCount); 
        form.AddField("entry.1965104821",playerRank); 




        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log(www.error);
            }
            else{
                Debug.Log("Form upload complete!");
            }
        }
    }
    //<input type="hidden" name="entry.337704600" value="">
    //<input type="hidden" name="entry.578045944" value="">
    //<input type="hidden" name="entry.666891163" value="">
    //<input type="hidden" name="entry.1115350779" value="">
    //<input type="hidden" name="entry.1128722234" value="">
    //<input type="hidden" name="entry.1053662980" value="">
    //<input type="hidden" name="entry.1965104821" value="">

    
}
