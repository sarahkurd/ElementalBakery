using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DefaultNamespace;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics; 

public enum PlayerPowerState
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
    public Sprite plateSprite;
    
    private float jumpForce = 12f;
    private float moveSpeed = 10f;
    private bool isOnIngredient = false;
    private GameObject currentCollidedIngredient;
    private bool isHoldingIngredient = false;
    private GameObject currentlyHoldingIngredient;
    
    //private int breakableGroundJumpCount = 0;
    private GameObject halfBrokenGround;
    private GameObject breakableLayer;
    private bool isBreakableLayer;

    private bool isFirstIngredientCollected = false; 
    private List<Sprite> spriteOrder;
    private bool isJumping = false;
    private bool isFacingRight = true;
    private PlayerPowerState currentPlayerState = PlayerPowerState.NEUTRAL;

    public GameObject collectAnalyticsObject;
    public GameObject SpriteManager;

    // Parameters for tracking the time for level 0 
    public float timeToGetIngredient; 
    private float levelZeroStartTime; 
    //private bool timing = false; 
    private bool activate;
    public GameObject tree, ice;
    private PlayerRanking playerRanking;
    private LevelCompletion levelCompletion;
    private bool isAtPlateStation = false;
    private bool hasPlate = false;
    private LevelManager levelManager;
    private bool isCollidedWithPlate;
    private GameObject currentCollidedPlate;
    private float flyTime = 1.0f;
    private float flyStartTime;
    private bool returnToGroundAfterFlying = false;
    private bool isAirJump = false;
    private float airForceUp = 45.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        playerRanking = GetComponent<PlayerRanking>();
        levelCompletion = GetComponent<LevelCompletion>();
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        
        //starting the timer for the level 
        //levelZeroStartTime = Time.time; 
       
        //timing = true;
    }

    // Update is called once per frame
    void Update()
    {
        // horizontal mechanics
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (!isAirJump)
        {
            if (isJumping) // slow down horizontal movement wheN player is in the air
            {
                transform.position += new Vector3(horizontalInput, 0, 0) * moveSpeed/1.3f * Time.deltaTime;

            }
            else
            {
                transform.position += new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;
            }  
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
        
        // vertical jump mechanics
        if (CanJump())
        {
            Jump();
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
        
        // logic for pick up an ingredient
        if (Input.GetKeyDown(KeyCode.Return) && isHoldingIngredient && isCollidedWithPlate)
        {
            PutIngredientOnPlate();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && isHoldingIngredient) // logic for drop ingredient
        {
            PlayerDropIngredientOrPlate();
        } else if (Input.GetKeyDown(KeyCode.Return) && isCollidedWithPlate)
        {
            PlayerPickUpPlate();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && isOnIngredient && !hasPlate)
        {
            PlayerPickUpIngredient(currentCollidedIngredient);
        }
        // logic for grabbing plate from plate station and placing plate on floor
        else if (Input.GetKeyDown(KeyCode.Return) && isAtPlateStation)
        {
            if (!isHoldingIngredient && !hasPlate)
            {
                GrabPlateFromPlateStation();
            }
        } 
        else if (Input.GetKeyDown(KeyCode.Return) && hasPlate)
        {
            PlayerDropIngredientOrPlate();
        }
    }

    private bool CanJump()
    {
        return Input.GetKeyDown(KeyCode.W) && IsGrounded();
    }

    void Jump()
    {
        isJumping = true;
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
        
        isJumping = false;
        returnToGroundAfterFlying = false;
        isAirJump = false;
    }

    private void EnableProgressBar(Collider2D other)
    {
         IngredientController ic = other.gameObject.GetComponentInParent<IngredientController>();
         ic.EnableProgressBar();
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.gameObject.CompareTag("Customer") && hasPlate)
        {
            Debug.Log("OnTrigger with customer");
            if (levelManager.CheckIfLevelComplete())
            {   //float timeToFinish =  Time.time - levelZeroStartTime;  
                OnLevelCompletion(); 
                //Debug.Log("Time to finish level: "+ timeToFinish+ " seconds");  

                //call the game over panel that shows "next level" button for level selection 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                //PlayManagerGame.isGameOver = true;
            }
        }
         //destroying the ingredient 
        if (other.gameObject.CompareTag("Ingredient"))
        {
            isOnIngredient = true;
            currentCollidedIngredient = other.gameObject;
            IngredientController ic = other.gameObject.GetComponentInParent<IngredientController>();
            if (ic.CanApplyPower(currentPlayerState)) // need to collide with correct power enabled
            {   
                if(isFirstIngredientCollected == false) {
                     //timeToGetIngredient =  Time.time - levelZeroStartTime; 
                     isFirstIngredientCollected = true; 
                }

                if (ic.currentIngredientState == IngredientCookingState.UNCOOKED || ic.currentIngredientState == IngredientCookingState.COOKING)
                {
                    //Debug.Log("Time to get Ingredient: " + timeToGetIngredient+ " seconds");  
                    EnableProgressBar(other); 
                }
            }
        }

        if (other.gameObject.CompareTag("Plates"))
        {
            Debug.Log("Collided with plate station");
            isAtPlateStation = true;
        }

        if (other.gameObject.CompareTag("plate"))
        {
            Debug.Log("Collided with plate");
            isCollidedWithPlate = true;
            currentCollidedPlate = other.gameObject;
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ingredient")) 
        {   
            isOnIngredient = false;
            //timer = 0f; 
        }
        
        if (other.gameObject.CompareTag("Plates"))
        {
            isAtPlateStation = false;
        }
        
        if (other.gameObject.CompareTag("plate"))
        {
            isCollidedWithPlate = false;
        }
    }

    public void OnLevelCompletion(){
        if (levelCompletion != null) {
            levelCompletion.OnLevelComplete();
        } else {
            Debug.LogError("LevelCompletion component not found!");
        }

        //float timeToFinish =  Time.time - levelZeroStartTime;  
        //CollectAnalytics analyticsScript = collectAnalyticsObject.GetComponent<CollectAnalytics>(); 
        
        //analyticsScript.putAnalytics(timeToFinish, timeToGetIngredient); 
    }

    private void OnLandedAir()
    {
        if (IsGrounded())
        {
            returnToGroundAfterFlying = false;
        }
        
        if (Input.GetKeyDown(KeyCode.S) && !returnToGroundAfterFlying)
        {
            flyStartTime = Time.time;
            isAirJump = true;
        }

        if (Input.GetKey(KeyCode.S) && !returnToGroundAfterFlying)
        {
            if (flyStartTime + flyTime >= Time.time)
            {
                rb.AddForce(Vector3.up * airForceUp * Time.deltaTime, ForceMode2D.Impulse);
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            returnToGroundAfterFlying = true;
            isAirJump = false;
        }
    }

    private void OnLandedIce()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            float scaleDirection = isFacingRight ? 1f : -1f;
            Vector3 effectPosition = transform.position + new Vector3(1.5f * scaleDirection, -1.0f, 0); // Adjust based on your needs
            GameObject effect = Instantiate(ice, effectPosition, Quaternion.identity);
            StartCoroutine(ScaleEffectX(effect, 10f, scaleDirection));
            Destroy(effect, 7f);
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

    private void PlayerPickUpIngredient(GameObject ingredientGameObject)
    {
        // get parent game object of the ingredient
        Debug.Log("Pick up ingredient with name: " + ingredientGameObject.name);
        Rigidbody2D rb = ingredientGameObject.GetComponent<Rigidbody2D>();
        BoxCollider2D bc = ingredientGameObject.GetComponent<BoxCollider2D>();
        bc.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Static; // so player can jump with ingredient
        rb.simulated = false;
        rb.constraints = RigidbodyConstraints2D.None;
        
        GameObject wholeGameObject = ingredientGameObject.transform.parent.gameObject;
        IngredientController ic = wholeGameObject.GetComponent<IngredientController>();
        ic.DisableProgressBar();
        wholeGameObject.transform.SetParent(this.gameObject.transform); // set the player game object as the parent of the ingredient
        if (!isFacingRight)
        {
            ingredientGameObject.transform.position = new Vector2(ingredientGameObject.transform.position.x - 1.0f,
                ingredientGameObject.transform.position.y);
        }
        else
        {
            ingredientGameObject.transform.position = new Vector2(ingredientGameObject.transform.position.x + 1.0f,
                ingredientGameObject.transform.position.y);
        }
        ingredientGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        isHoldingIngredient = true;
        currentlyHoldingIngredient = ingredientGameObject;
    }

    private void PlayerDropIngredientOrPlate()
    {
        // the player no longer has a nested ingredient game object. Move the game object back to the 
        // root of the scene.
        if (hasPlate)
        {
            Debug.Log("Drop plate");
            GameObject plate = this.gameObject.transform.GetChild(0).gameObject;
            Rigidbody2D rb = plate.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            hasPlate = false;
        } 
        else if (isHoldingIngredient)
        {
            Debug.Log("Drop ingredient");
            Rigidbody2D rb = currentlyHoldingIngredient.GetComponent<Rigidbody2D>();
            currentlyHoldingIngredient.transform.localScale = new Vector3(1f, 1f, 1f);
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            isHoldingIngredient = false;
        }
        this.gameObject.transform.DetachChildren();
    }

    private void GrabPlateFromPlateStation()
    {
        GameObject plateGameObject = new GameObject();
        plateGameObject.tag = "plate";
        plateGameObject.AddComponent<SpriteRenderer>();
        SpriteRenderer sr = plateGameObject.GetComponent<SpriteRenderer>();
        plateGameObject.AddComponent<BoxCollider2D>();
        plateGameObject.AddComponent<Rigidbody2D>();
        plateGameObject.AddComponent<PlateController>();
        Rigidbody2D rb = plateGameObject.GetComponent<Rigidbody2D>();
        BoxCollider2D bc = plateGameObject.GetComponent<BoxCollider2D>();
        bc.size = new Vector2(2.5f, 1.0f);
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        sr.sprite = plateSprite;
        sr.sortingLayerName = "ingredients";
        
        plateGameObject.transform.SetParent(this.gameObject.transform);
        Vector3 playerPostion = gameObject.transform.position;
        plateGameObject.transform.position = new Vector3(playerPostion.x + 2.0f, playerPostion.y, playerPostion.z);
        hasPlate = true;
    }

    private void PutIngredientOnPlate()
    {
        Debug.Log("Put ingredient on plate");
        currentlyHoldingIngredient.GetComponent<SpriteRenderer>().sortingOrder = 1;
        Destroy(currentlyHoldingIngredient.GetComponent<BoxCollider2D>());
        currentlyHoldingIngredient.transform.position =
            new Vector2(currentCollidedPlate.transform.position.x, currentCollidedPlate.transform.position.y + 1.0f);
        currentlyHoldingIngredient.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        GameObject wholeGameObject = currentlyHoldingIngredient.transform.parent.gameObject;
        wholeGameObject.transform.SetParent(currentCollidedPlate.transform);
        isHoldingIngredient = false;
        
        // add this ingredient to the players list of collected items/ ingredients that are on the plate
        IngredientController ic = currentlyHoldingIngredient.GetComponentInParent<IngredientController>();
        levelManager.PlateIngredient(currentlyHoldingIngredient.name, ic.currentIngredientState);
    }

    private void PlayerPickUpPlate()
    {
        Debug.Log("Player pick up plate");
        Rigidbody2D rb = currentCollidedPlate.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        BoxCollider2D bc = currentCollidedPlate.GetComponent<BoxCollider2D>();
        bc.isTrigger = false;
        currentCollidedPlate.transform.SetParent(this.gameObject.transform);
        Vector3 playerPostion = gameObject.transform.position;
        currentCollidedPlate.transform.position = new Vector3(playerPostion.x + 2.0f, playerPostion.y, playerPostion.z);
        hasPlate = true;
    }

}
 