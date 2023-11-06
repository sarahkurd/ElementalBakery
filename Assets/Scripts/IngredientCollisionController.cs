using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCollisionController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
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
            rb.bodyType = RigidbodyType2D.Static;
            bc.isTrigger = true;
        }
    }
}
