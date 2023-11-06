using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCollisionController : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IngredientCollisionController Start()");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("IngredientCollisionController OnCollisionEnter2D");
        if (!other.gameObject.CompareTag("Player"))
        {
            Debug.Log("IngredientCollisionController OnCollisionEnter2D NOT COLLIDING WITH PLAYER");
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
