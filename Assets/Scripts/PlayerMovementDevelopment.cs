using System;
using System.Collections.Generic;
using UnityEngine;

enum PlayerPowerState
{
        FIRE_RIGHT, FIRE_ACTIVE, FIRE_LEFT, FIRE_TOP
}

public class PlayerMovementDevelopment : MonoBehaviour
{
    // ----- Components ----
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask breakableGround;
    
    public float jumpForce = 8f;
    public float moveSpeed = 10f;
    private bool isOnIngredient = false;
    private bool wasGrounded = true;
    public GameObject uiObjectToShow;
    private int breakableGroundJumpCount = 0;
    private bool isOnBreakableGround = false;
    private GameObject halfBrokenGround;


    private List<Sprite> spriteOrder;
    private float timer = 0f;
    public float destroyTime = 5f;
    private bool isJumping = false;
    
    private PlayerPowerState currentPlayerState = PlayerPowerState.FIRE_RIGHT;
    private List<string> collected = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        //spriteOrder = new List<Sprite>() { powerRight, powerBottom, powerLeft, powerTop };
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
        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || isOnIngredient))
        {
            Jump();
        }

        // animations for moving left/right and jumping
        if (horizontalInput > 0f && !isJumping)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", true);
        } else if (horizontalInput < 0f && !isJumping)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isMovingLeft", true);
            animator.SetBool("isMovingRight", false);
        }
        else
        {
            animator.SetBool("isMovingRight", false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isIdle", true);
        }

        // rotate player mechanics
        SetCurrentSpriteOnRotation();

        //timer if player lands on ingredient
        wasGrounded = currentlyGrounded;

    }

    void Jump()
    {
        isJumping = true;
        animator.SetBool("isIdle", true);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void SetCurrentSpriteOnRotation()
    {   if (!IsGrounded()){
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                switch (currentPlayerState)
                {
                    case PlayerPowerState.FIRE_RIGHT:
                        animator.SetBool("isFireTop", true);
                        animator.SetBool("isFireRight", false);
                        animator.SetBool("isFireActive", false);
                        animator.SetBool("isFireLeft", false);
                        currentPlayerState = PlayerPowerState.FIRE_TOP;
                        break;
                    case PlayerPowerState.FIRE_ACTIVE:
                        animator.SetBool("isFireRight", true);
                        animator.SetBool("isFireTop", false);
                        animator.SetBool("isFireActive", false);
                        animator.SetBool("isFireLeft", false);
                        currentPlayerState = PlayerPowerState.FIRE_RIGHT;
                        break;
                    case PlayerPowerState.FIRE_LEFT:
                        animator.SetBool("isFireActive", true);
                        animator.SetBool("isFireRight", false);
                        animator.SetBool("isFireTop", false);
                        animator.SetBool("isFireLeft", false);
                        currentPlayerState = PlayerPowerState.FIRE_ACTIVE;
                        break;
                    case PlayerPowerState.FIRE_TOP:
                        animator.SetBool("isFireLeft", true);
                        animator.SetBool("isFireActive", false);
                        animator.SetBool("isFireRight", false);
                        animator.SetBool("isFireTop", false);
                        currentPlayerState = PlayerPowerState.FIRE_LEFT;
                        break;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                switch (currentPlayerState)
                {
                    case PlayerPowerState.FIRE_RIGHT:
                        animator.SetBool("isFireActive", true);
                        animator.SetBool("isFireRight", false);
                        animator.SetBool("isFireTop", false);
                        animator.SetBool("isFireLeft", false);
                        currentPlayerState = PlayerPowerState.FIRE_ACTIVE;
                        break;
                    case PlayerPowerState.FIRE_ACTIVE:
                        animator.SetBool("isFireLeft", true);
                        animator.SetBool("isFireActive", false);
                        animator.SetBool("isFireRight", false);
                        animator.SetBool("isFireTop", false);
                        currentPlayerState = PlayerPowerState.FIRE_LEFT;
                        break;
                    case PlayerPowerState.FIRE_LEFT:
                        animator.SetBool("isFireTop", true);
                        animator.SetBool("isFireRight", false);
                        animator.SetBool("isFireActive", false);
                        animator.SetBool("isFireLeft", false);
                        currentPlayerState = PlayerPowerState.FIRE_TOP;
                        break;
                    case PlayerPowerState.FIRE_TOP:
                        animator.SetBool("isFireRight", true);
                        animator.SetBool("isFireActive", false);
                        animator.SetBool("isFireLeft", false);
                        animator.SetBool("isFireTop", false);
                        currentPlayerState = PlayerPowerState.FIRE_RIGHT;
                        break;
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
        if (isBreakableLayer && isJumping && currentPlayerState == PlayerPowerState.FIRE_ACTIVE)
        {
            breakableGroundJumpCount++;
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

        //destroying the ingredient 
        if (other.gameObject.CompareTag("Ingredient"))
        {
            isOnIngredient = true;
            if (currentPlayerState == PlayerPowerState.FIRE_ACTIVE)
            {
                uiObjectToShow.SetActive(true);
                
                Destroy(other.gameObject, 2);
                // add this ingredient with its name to the list of collected items
                collected.Add(other.gameObject.name);
            }
        }

        if (other.gameObject.CompareTag("Customer"))
        {
            if (collected.Contains("Chicken"))
            {
                Debug.Log("Finished");
            }
        }
        
        isJumping = false;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ingredient")) 
        {   
            isOnIngredient = false;
            timer = 0f; 
        }
    }

}
