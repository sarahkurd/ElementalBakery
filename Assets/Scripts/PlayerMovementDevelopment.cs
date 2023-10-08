using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovementDevelopment : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    [SerializeField] private bool isDescending;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask breakableGround;
    [SerializeField] private LayerMask breakableGroundStageTwo;
    
    private enum MovementState { idle, running, jumping, falling }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // horizontal mechanics
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(7f * horizontalInput, rb.velocity.y);
        
        // vertical jump mechanics
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(0, 7);
        }
        
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            .1f,
            jumpableGround
        ) ||Physics2D.BoxCast(
                   boxCollider.bounds.center,
                   boxCollider.bounds.size,
                   0f,
                   Vector2.down,
                   .1f,
                   breakableGround
               );
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        bool isBreakableLayer = other.gameObject.layer == LayerMask.NameToLayer("Breakable");
        Debug.Log(isBreakableLayer);
        if (isBreakableLayer)
        {
            Destroy(other.gameObject);
        }
    }
}
