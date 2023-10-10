using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovementDevelopment : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isDescending;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask breakableGround;
    [SerializeField] private Sprite powerLeft;
    [SerializeField] private Sprite powerBottom;
    [SerializeField] private Sprite powerRight;
    [SerializeField] private Sprite powerTop;
    private Sprite currentSprite;
    private enum MovementState { idle, running, jumping, falling }

    private List<Sprite> spriteOrder;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = spriteRenderer.sprite;
        spriteOrder = new List<Sprite>() { powerRight, powerBottom, powerLeft, powerTop };
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
        
        // rotate player mechanics
        bool leftKeyInput = Input.GetKeyUp("left");
        bool rightKeyInput = Input.GetKeyUp("right");
        setCurrentSprite(leftKeyInput, rightKeyInput);
        spriteRenderer.sprite = currentSprite; // update the sprite in SpriteRenderer component
    }

    private void setCurrentSprite(bool leftKeyInput, bool rightKeyInput)
    {
        if (leftKeyInput)
        {
            int index = spriteOrder.IndexOf(currentSprite);
            if (index == 0)
            {
                currentSprite = spriteOrder[spriteOrder.Count - 1];
            }
            else
            {
                currentSprite = spriteOrder[index - 1];
            }
        }
        else if (rightKeyInput)
        {
            int index = spriteOrder.IndexOf(currentSprite);
            if (index == spriteOrder.Count - 1)
            {
                currentSprite = spriteOrder[0];
            }
            else
            {
                currentSprite = spriteOrder[index + 1];
            }
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
        if (isBreakableLayer)
        {
            Destroy(other.gameObject);
        }
    }
}
