using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DefaultNamespace;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics; 

enum PlayerPowerState
{
    FIRE_ACTIVE, WATER_ACTIVE, AIR_ACTIVE, NEUTRAL
}

public class PlayerMovementDevelopment : MonoBehaviour
{
    // ----- Components ----
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask breakableGround;
    
    private float jumpForce = 15f;
    private float moveSpeed = 10f;
    private bool isOnIngredient = false;
    
    private int breakableGroundJumpCount = 0;
    private GameObject halfBrokenGround;
    private GameObject breakableLayer;
    private bool isBreakableLayer;

    private bool isFirstIngredientCollected = false; 
    private List<Sprite> spriteOrder;
    private float timer = 0f;
    private bool isJumping = false;
    private const int MAX_JUMPS = 1;
    private int jumpsLeft = MAX_JUMPS;
    private bool isFacingRight = true;
    private float airForce = 12f;
    private int airJumpCount = 0;
    private int MAX_AIR_JUMP = 3;
    private PlayerPowerState currentPlayerState = PlayerPowerState.NEUTRAL;

    public GameObject collectAnalyticsObject; 
    

    // Parameters for tracking the time for level 0 
    public float timeToGetIngredient; 
    private float levelZeroStartTime; 
    private bool timing = false; 
    private bool activate;
    public GameObject tree, ice;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

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
        if (isJumping) // slow down horizontal movement wheN player is in the air
        {
            transform.position += new Vector3(horizontalInput, 0, 0) * moveSpeed/1.3f * Time.deltaTime;

        }
        else
        {
            transform.position += new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;
        }
        
        // vertical jump mechanics
        if (CanJump())
        {
            Jump();
        }

        // animations for moving left/right and jumping
        if (horizontalInput > 0f && !isJumping)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", true);
            isFacingRight = true;
        } else if (horizontalInput < 0f && !isJumping)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isMovingLeft", true);
            animator.SetBool("isMovingRight", false);
            isFacingRight = false;
        }
        else
        {
            animator.SetBool("isMovingRight", false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isIdle", true);
        }

        // rotate player mechanics
        SetCurrentSpriteOnRotation();
        
        // logic for breakable grounds
        if (isBreakableLayer && IsGrounded() && currentPlayerState == PlayerPowerState.FIRE_ACTIVE && Input.GetKey(KeyCode.S))
        {
            Destroy(breakableLayer);
        }
        
        // logic for air and water power activation
        if(PlayerPowerState.AIR_ACTIVE == currentPlayerState)
        {
            OnLandedAir();
        }
        else if(PlayerPowerState.WATER_ACTIVE == currentPlayerState && IsGrounded())
        {
            OnLandedIce();
        }

        if (timing){
            float elapsedTime = Time.time - levelZeroStartTime; 
        }

    }

    private bool CanJump()
    {
        return Input.GetKeyDown(KeyCode.W) && IsGrounded();
    }

    void Jump()
    {
        isJumping = true;
        //animator.SetBool("isIdle", true);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void SetCurrentSpriteOnRotation()
    {   
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                switch (currentPlayerState)
                {
                    case PlayerPowerState.NEUTRAL:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireTop", true);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.AIR_ACTIVE;
                            break; 
                        }
                        else
                        {
                            animator.SetBool("isFireTop", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireActive", true);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.FIRE_ACTIVE;
                            break;
                        }
                    case PlayerPowerState.FIRE_ACTIVE:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireRight", true);
                            animator.SetBool("isFireTop", false);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.NEUTRAL;
                            break;
                        }
                        else
                        {
                            animator.SetBool("isFireRight", true);
                            animator.SetBool("isFireTop", false);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.WATER_ACTIVE;
                            break;
                        }
                        
                    case PlayerPowerState.WATER_ACTIVE:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireActive", true);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", false);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.FIRE_ACTIVE;
                            break;
                        }
                        else
                        {
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", true);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.AIR_ACTIVE;
                            break;
                        }
                    
                    case PlayerPowerState.AIR_ACTIVE:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireLeft", true);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", false);
                            currentPlayerState = PlayerPowerState.WATER_ACTIVE;
                            break;
                        }
                        else
                        {
                            animator.SetBool("isFireLeft", true);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", false);
                            currentPlayerState = PlayerPowerState.NEUTRAL;
                            break;
                        }
                    
                }
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                switch (currentPlayerState)
                {
                    case PlayerPowerState.NEUTRAL:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireActive", true);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", false);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.FIRE_ACTIVE;
                            break;
                        }
                        else
                        {
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", true);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.AIR_ACTIVE;
                            break;
                        }
                    case PlayerPowerState.FIRE_ACTIVE:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireLeft", true);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", false);
                            currentPlayerState = PlayerPowerState.WATER_ACTIVE;
                            break; 
                        }
                        else
                        {
                            animator.SetBool("isFireLeft", true);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireTop", false);
                            currentPlayerState = PlayerPowerState.NEUTRAL;
                            break; 
                        }
                    case PlayerPowerState.WATER_ACTIVE:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireTop", true);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.AIR_ACTIVE;
                            break;
                        }
                        else
                        {
                            animator.SetBool("isFireTop", false);
                            animator.SetBool("isFireRight", false);
                            animator.SetBool("isFireActive", true);
                            animator.SetBool("isFireLeft", false);
                            currentPlayerState = PlayerPowerState.FIRE_ACTIVE;
                            break;
                        }
                    case PlayerPowerState.AIR_ACTIVE:
                        if (isFacingRight)
                        {
                            animator.SetBool("isFireRight", true);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireLeft", false);
                            animator.SetBool("isFireTop", false);
                            currentPlayerState = PlayerPowerState.NEUTRAL;
                            break;
                        }
                        else
                        {
                            animator.SetBool("isFireRight", true);
                            animator.SetBool("isFireActive", false);
                            animator.SetBool("isFireLeft", false);
                            animator.SetBool("isFireTop", false);
                            currentPlayerState = PlayerPowerState.WATER_ACTIVE;
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
        isBreakableLayer = other.gameObject.layer == LayerMask.NameToLayer("Breakable");
        if (isBreakableLayer)
        {
            breakableLayer = other.gameObject;
        }

        //destroying the ingredient 
        if (other.gameObject.CompareTag("Ingredient"))
        {
            isOnIngredient = true;
            if (currentPlayerState == PlayerPowerState.FIRE_ACTIVE)
            {   
                if(isFirstIngredientCollected == false) {
                     timeToGetIngredient =  Time.time - levelZeroStartTime; 
                     isFirstIngredientCollected = true; 
                }

                IngredientController ic = other.gameObject.GetComponentInParent<IngredientController>();
                if (ic.currentIngredientState == IngredientCookingState.COMPLETE)
                {
                    PlayerItems.collected.Add(other.gameObject.name);
                    ic.DestroyIngredientAndProgressBar();
                } else if (ic.currentIngredientState == IngredientCookingState.UNCOOKED)
                {
                    //Debug.Log("Time to get Ingredient: " + timeToGetIngredient+ " seconds");  
                    EnableProgressBar(other); 
                }
            }
        }
        
        isJumping = false;
        airJumpCount = 0; //reset possible air jump count
    }

    private void EnableProgressBar(Collision2D other)
    {
         IngredientController ic = other.gameObject.GetComponentInParent<IngredientController>();
         ic.EnableProgressBar();
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
            if (PlayerItems.collected.Contains("lowerBread") && PlayerItems.collected.Contains("upperBread") && PlayerItems.collected.Contains("meat"))
            {
                // exit scene to be added
                //Debug.Log("Exit Game");
                OnLevelCompletion(); 
                PlayManagerGame.isGameOver = true;


            }
            if (PlayerItems.collected.Contains("chicken"))
            {   //float timeToFinish =  Time.time - levelZeroStartTime;  
                OnLevelCompletion(); 
                //Debug.Log("Time to finish level: "+ timeToFinish+ " seconds");  

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        
    }

    public void OnLevelCompletion(){

        float timeToFinish =  Time.time - levelZeroStartTime;  
        CollectAnalytics analyticsScript = collectAnalyticsObject.GetComponent<CollectAnalytics>(); 
        
        analyticsScript.putAnalytics(timeToFinish, timeToGetIngredient); 

        int activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
         
    }

    private void OnLandedAir()
    {
        if (Input.GetKeyDown(KeyCode.S) && airJumpCount < MAX_AIR_JUMP)
        {
            rb.AddForce(Vector3.up * airForce, ForceMode2D.Impulse);
            airJumpCount++;
        }
    }

    private IEnumerator ScaleEffectY(GameObject obj, float targetScaleY)
    {
        float duration = 0.4f; // Time to scale over
        float elapsedTime = 0f;
        Vector3 initialScale = obj.transform.localScale;
        Vector3 targetScale = new Vector3(initialScale.x, targetScaleY, initialScale.z);

        while (elapsedTime < duration)
        {
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = targetScale; // Ensure the object reaches the target scale at the end
    }

    private void OnLandedIce()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            float scaleDirection = isFacingRight ? 1f : -1f;
            Vector3 effectPosition = transform.position + new Vector3(1.5f * scaleDirection, -1.1f, 0); // Adjust based on your needs
            GameObject effect = Instantiate(ice, effectPosition, Quaternion.identity);
            StartCoroutine(ScaleEffectX(effect, 7f, scaleDirection));
            Destroy(effect, 5f);
        }
    }

    private IEnumerator ScaleEffectX(GameObject obj, float targetScaleX, float scaleDirection)
    {
        float duration = 0.4f; // Time to scale over
        float elapsedTime = 0f;

        // Set the object's rotation to match the player's forward direction

        Vector3 initialScale = obj.transform.localScale;
        Vector3 targetScale = new Vector3(targetScaleX * scaleDirection, initialScale.y, initialScale.z);

        while (elapsedTime < duration)
        {
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = targetScale;
    }

}
 