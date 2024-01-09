using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    private BoxCollider2D bc;

    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("PlateController OnCollisionEnter2D");
            rb.bodyType = RigidbodyType2D.Static;
            bc.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }
}
