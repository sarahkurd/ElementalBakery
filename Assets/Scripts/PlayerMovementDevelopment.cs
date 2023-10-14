using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovementDevelopment : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce = 4f;
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
    private enum MovementState { idle, running, jumping, falling };
    
    
    public float moveSpeed = 10f;
    private bool isOnObject = false;
    private bool wasGrounded = true;
    private int breakableGroundJumpCount = 0;
    private bool isOnBreakableGround = false;
    private GameObject halfBrokenGround;


    private List<Sprite> spriteOrder;
    private float timer = 0f;
    public float destroyTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = spriteRenderer.sprite;
        spriteOrder = new List<Sprite>() { powerRight, powerBottom, powerLeft, powerTop };
        halfBrokenGround = GameObject.Find("Half-Broken");
        halfBrokenGround.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool currentlyGrounded = IsGrounded();
        // horizontal mechanics
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;

        // vertical jump mechanics
        if ((Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space)) && IsGrounded())
        {
            Jump();
        }

        // rotate player mechanics
        setCurrentSprite();
        spriteRenderer.sprite = currentSprite; // update the sprite in SpriteRenderer component

        //timer if player lands on ingredient
        wasGrounded = currentlyGrounded;

    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        if (isOnBreakableGround)
        {
            breakableGroundJumpCount++;
            Debug.Log(breakableGroundJumpCount);
        }
    }

    private void setCurrentSprite()
    {   if (!IsGrounded()){
            if (Input.GetKeyDown(KeyCode.LeftArrow))
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
            else if (Input.GetKeyDown(KeyCode.RightArrow))
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
        // Logic to break the breakable ground with fire side 
        bool isBreakableLayer = other.gameObject.layer == LayerMask.NameToLayer("Breakable");
        if (isBreakableLayer)
        {
            isOnBreakableGround = true;
            if (breakableGroundJumpCount == 1)
            {
                if (halfBrokenGround.activeSelf)
                {
                    Destroy(halfBrokenGround);
                }
                else
                {
                    // update the layer to show a half broken ground
                    Destroy(other.gameObject);
                    halfBrokenGround.SetActive(true);
                    breakableGroundJumpCount = 0; 
                }
            }
        }
        else
        {
            isOnBreakableGround = false;
            breakableGroundJumpCount = 0;
        }

        // destroying the ingredient 
        if (other.gameObject.CompareTag("Ingredient"))
        {
            isOnObject = true;
            if (currentSprite == powerBottom)
            {
                Destroy(other.gameObject, 2);
            }
           
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ingredient")) 
        {
            isOnObject = false;
            timer = 0f; 
        }
    }

}
