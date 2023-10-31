using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowHintController : MonoBehaviour
{
    public GameObject canvas;

    public void CloseCanvas()
    {
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Canvas is not assigned!");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
