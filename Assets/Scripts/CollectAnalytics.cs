using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking; 
using UnityEngine.SceneManagement; 

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
        
    
    public static void putAnalytics(float finishTime, float timeToGetFirstIngredient ){

       StartCoroutine(Post("23425sess1", finishTime.ToString(), timeToGetFirstIngredient.ToString() )); 


    }

    private static IEnumerator Post(string sessionId, string finishTime, string timeToGetFirstIngredient){
        WWWForm form = new WWWForm(); 
        form.AddField("entry.337704600", sessionId ); 
        form.AddField("entry.666891163", finishTime); 
        form.AddField("entry.1115350779", timeToGetFirstIngredient); 

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


    
}
