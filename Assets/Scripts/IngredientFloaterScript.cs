using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientFloaterScript : MonoBehaviour
{   public float floatSpeed = 1f;  // Adjust this value to control the speed of floating
    public float floatHeight = 0.05f;
    public Vector3 startPosition;

    public GameObject IngredientList; 
    private Collider2D childCollider;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        childCollider = GetComponentInChildren<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {      if (childCollider.IsTouchingLayers(LayerMask.GetMask("ground"))){
        Float(); 
    }
        
    }

    void Float(){
        float verticalOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Update the position of the GameObject
        transform.position = startPosition + new Vector3(0f, verticalOffset, 0f);
    }
}
