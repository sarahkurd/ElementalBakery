using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public float jumpForce = 10f;
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

    // Parameters for tracking the time for level 0 

    private float levelZeroStartTime; 
    private bool timing = false; 
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        //spriteOrder = new List<Sprite>() { powerRight, powerBottom, powerLeft, powerTop };
        halfBrokenGround = GameObject.Find("Half-Broken");
        halfBrokenGround.SetActive(false);

        //starting the timer for the level 
        levelZeroStartTime = Time.time; 
        timing = true; 
        
    }

    // Update is called once per frame
    void Update()
    {
        bool currentlyGrounded = IsGrounded();
        // horizontal mechanics
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;
        // vertical jump mechanics
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && (IsGrounded() || isOnIngredient))
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

        if (timing){
            float elapsedTime = Time.time - levelZeroStartTime; 
        }

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

                float timeToGetIngredient =  Time.time - levelZeroStartTime; 
                Debug.Log("Time to get Ingredient: " + timeToGetIngredient+ " seconds");  
                EnableProgressBar(other); 
                // uiObjectToShow.SetActive(true);
                Destroy(other.gameObject, 2.5f);
                // add this ingredient with its name to the list of collected items
                collected.Add(other.gameObject.name); 
            }
        }
        
        isJumping = false;
    }

    private void EnableProgressBar(Collision2D other){ 
         uiObjectToShow.SetActive(true); 
         RectTransform uiRectTransform = uiObjectToShow.GetComponent<RectTransform>();

         Vector3 referencePosition = other.gameObject.transform.position; 
         Vector2 newAnchoredPosition = new Vector2(7f, 7f);

        uiRectTransform.anchoredPosition = newAnchoredPosition; 


       

    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ingredient")) 
        {   
            isOnIngredient = false;
            timer = 0f; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.gameObject.CompareTag("Customer"))
        {
            //foreach (var x in collected)
            //{
            //    Debug.Log(x.ToString());
            //}
            if (collected.Contains("lowerBread") && collected.Contains("upperBread") && collected.Contains("meat"))
            {
                // exit scene to be added
                Debug.Log("Exit Game");
            }
            if (collected.Contains("Chicken"))
            {   float timeToFinish =  Time.time - levelZeroStartTime;  

                Debug.Log("Time to finish level: "+ timeToFinish+ " seconds");  

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        
    }

}
