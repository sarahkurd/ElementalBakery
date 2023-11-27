using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Numerics;
using DefaultNamespace;
using models;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UI;

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
    
    private float jumpForce = 15f;
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
    private bool isApplyingPowerToCook = false;
    private bool isAtStove;
    private bool isAtSink;
    private int maxIcePlatforms=2;
    private int currIcePlatforms=0;
    public GameObject[] powerVfx;
    private bool bananaCollision;
    public PowerProgressBar powerProgressBar;
    public GameObject powerProgressMask;
    public GameObject ingredientList; 


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
        if(currentPlayerState == PlayerPowerState.WATER_ACTIVE || currentPlayerState == PlayerPowerState.AIR_ACTIVE)
        {
            powerProgressMask.SetActive(true);
            if(currentPlayerState == PlayerPowerState.WATER_ACTIVE)
            {
                powerProgressBar.SetPowerValue(maxIcePlatforms - currIcePlatforms, maxIcePlatforms);
            }
            else
            {
                powerProgressBar.SetPowerValue(flyTime - (Time.time - flyStartTime), flyTime);
                //Debug.Log("Fly time: " + (flyTime - (Time.time - flyStartTime)));
                if(flyTime - (Time.time - flyStartTime)<0)
                {
                    powerProgressMask.SetActive(false);
                }
            }
        }
        else
        {
            powerProgressMask.SetActive(false);
        }
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

        if(currentPlayerState == PlayerPowerState.FIRE_ACTIVE && IsCommandKey())
        {
            GameObject fire = Instantiate(powerVfx[0], transform.position, Quaternion.identity);
            Destroy(fire, 2f);
        }
        else if(currentPlayerState == PlayerPowerState.WATER_ACTIVE && IsCommandKey())
        {
            GameObject mist = Instantiate(powerVfx[1], transform.position, Quaternion.identity);
            Destroy(mist, 2f);
        }
        else if(currentPlayerState == PlayerPowerState.AIR_ACTIVE && IsCommandKey())
        {
            GameObject steam = Instantiate(powerVfx[2], transform.position, Quaternion.identity);
            Destroy(steam, 2f);
        }
        
        // logic for breakable grounds
        if (isBreakableLayer && IsGrounded() && currentPlayerState == PlayerPowerState.FIRE_ACTIVE && IsCommandKey())
        {
            Destroy(breakableLayer);
        }
        

        // Use S to cook ingredients when you collide with them
        if (IsCommandKey() && isOnIngredient)
        {
            IngredientController ic = currentCollidedIngredient.GetComponentInParent<IngredientController>();
            if (ic.CanApplyPower(currentPlayerState)) // need to collide with correct power enabled
            {
                isApplyingPowerToCook = true;
                if(isFirstIngredientCollected == false) {
                    //timeToGetIngredient =  Time.time - levelZeroStartTime; 
                    isFirstIngredientCollected = true; 
                }

                if (ic.currentIngredientState == IngredientCookingState.UNCOOKED || ic.currentIngredientState == IngredientCookingState.COOKING)
                {
                    //Debug.Log("Time to get Ingredient: " + timeToGetIngredient+ " seconds");  
                    ic.EnableProgressBar();
                }
            }
        } 
        else if (Input.GetKeyDown(KeyCode.Return) && isHoldingIngredient && isAtStove)
        {
            PlaceItemOnStove();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && isHoldingIngredient && isAtSink)
        {
            PlaceItemOnSink();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && isAtStove)
        {
            PickUpItemFromStove();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && isAtSink)
        {
            PickUpItemFromSink();
        }
        // logic for pick up an ingredient or plate or drop items
        else if (Input.GetKeyDown(KeyCode.Return) && isHoldingIngredient && isCollidedWithPlate)
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
        else if (Input.GetKeyDown(KeyCode.Return) && isOnIngredient && !hasPlate && !isHoldingIngredient)
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
        
        // logic for air and water power activation
        if(PlayerPowerState.AIR_ACTIVE == currentPlayerState && !isApplyingPowerToCook)
        {
            if(IsCommandKey())
            {
                GameObject steam = Instantiate(powerVfx[2], transform.position, Quaternion.identity);
                Destroy(steam, 2f);
            }
            OnLandedAir();
        }
        else if(PlayerPowerState.WATER_ACTIVE == currentPlayerState && IsGrounded() && !isApplyingPowerToCook)
        {
            if(Input.GetKeyDown(KeyCode.E) && currIcePlatforms < maxIcePlatforms )
            {
                OnLandedIce();
            }
        }

        if (bananaCollision)
        {
            this.gameObject.transform.Rotate(0.0f, 0.0f, 1.0f, Space.World);
        }
    }

    private bool IsCommandKey()
    {   return Input.GetKey(KeyCode.E); 
        //return Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
    }

    private bool CanJump()
    {
        return (Input.GetKeyDown(KeyCode.Space) ||Input.GetKeyDown(KeyCode.W))  && IsGrounded();
    }

    void Jump()
    {
        isJumping = true;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void SetCurrentSpriteOnRotation()
    {
        // rotate powers clockwise on space bar press
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
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

    private void OnCollisionStay2D(Collision2D other)
    {
        // Logic to break the breakable ground with fire side 
        isBreakableLayer = other.gameObject.layer == LayerMask.NameToLayer("Breakable");
        if (isBreakableLayer)
        {
            breakableLayer = other.gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        isBreakableLayer = other.gameObject.layer == LayerMask.NameToLayer("Breakable");
        bool isGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");

        if ((isGround || isBreakableLayer) && bananaCollision)
        {
            this.gameObject.transform.rotation = Quaternion.identity;
            bananaCollision = false;
        }
        
        // move the player back in the x and y direction like they are slipping back
        if (other.gameObject.CompareTag("Banana"))
        {
            Debug.Log("Banana collision");
            bananaCollision = true;
            if (isFacingRight)
            {
                rb.AddForce(new Vector2(-10.0f, 15.0f), ForceMode2D.Impulse);
            }
            else 
            {
                rb.AddForce(new Vector2(10.0f, 15.0f), ForceMode2D.Impulse);
            }
        }
        
        isJumping = false;
        returnToGroundAfterFlying = false;
        isAirJump = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Customer") && hasPlate)
        {
            Debug.Log("OnTrigger with customer");
            if (levelManager.CheckIfLevelComplete())
            {    
                OnLevelCompletion();
            }
        }
        
        // mark that player collided with ingredient
        if (other.gameObject.CompareTag("Ingredient"))
        {
            isOnIngredient = true;
            //Debug.Log("On Ingredient!");
            currentCollidedIngredient = other.gameObject;
        } 
        else if (other.gameObject.CompareTag("Plates"))
        {
            Debug.Log("Collided with plate station");
            isAtPlateStation = true;
        } 
        else if (other.gameObject.CompareTag("plate"))
        {
            Debug.Log("Collided with plate");
            isCollidedWithPlate = true;
            currentCollidedPlate = other.gameObject;
        } 
        else if (other.gameObject.CompareTag("Stove"))
        {
            isAtStove = true;
        } 
        else if (other.gameObject.CompareTag("Sink"))
        {
            isAtSink = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ingredient")) 
        {   
            isOnIngredient = false;
            IngredientController ic = currentCollidedIngredient.GetComponentInParent<IngredientController>();
            isApplyingPowerToCook = false;
            ic.DisableProgressBar(); //stop cooking when player leaves contact with the ingredient
            //timer = 0f; 
        }
        
        else if (other.gameObject.CompareTag("Plates"))
        {
            isAtPlateStation = false;
        }
        
        else if (other.gameObject.CompareTag("plate"))
        {
            isCollidedWithPlate = false;
        }
        else if (other.gameObject.CompareTag("Stove"))
        {
            isAtStove = false;
        } 
        else if (other.gameObject.CompareTag("Sink"))
        {
            isAtSink = false;
        }
    }

    public void OnLevelCompletion(){
        if (levelCompletion != null) {
            levelCompletion.OnLevelComplete();
        } else {
            Debug.LogError("LevelCompletion component not found!");
        }
    }

    private void OnLandedAir()
    {
        if (IsGrounded())
        {
            returnToGroundAfterFlying = false;
        }
        
        if ((Input.GetKeyDown(KeyCode.E)) && !returnToGroundAfterFlying)
        {
            flyStartTime = Time.time;
            isAirJump = true;
            returnToGroundAfterFlying = true;
        }

        if (IsCommandKey())
        {
            if (flyStartTime + flyTime >= Time.time)
            {
                rb.AddForce(Vector3.up * airForceUp * Time.deltaTime, ForceMode2D.Impulse);
            }
        }

        if ((Input.GetKeyDown(KeyCode.E)))
        {
            isAirJump = false;
        }
    }

    private void OnLandedIce()
    {
        if (IsCommandKey())
        {
            float scaleDirection = isFacingRight ? 1f : -1f;
            Vector3 effectPosition = transform.position + new Vector3(1.5f * scaleDirection, -1.0f, 0); // Adjust based on your needs
            GameObject effect = Instantiate(ice, effectPosition, Quaternion.identity);
            currIcePlatforms++;
            StartCoroutine(ScaleEffectX(effect, 10f, scaleDirection));            
            StartCoroutine(DestroyPrefabAfterDelay(effect, 7f)); // Destroy after 5 seconds
        }
    }

    System.Collections.IEnumerator DestroyPrefabAfterDelay(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(prefab);
        currIcePlatforms--; // Decrement the counter
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
        //Debug.Log("Pick up ingredient with name: " + ingredientGameObject.name);
        Rigidbody2D rb = ingredientGameObject.GetComponent<Rigidbody2D>();
        BoxCollider2D bc = ingredientGameObject.GetComponent<BoxCollider2D>();
        bc.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Static; // so player can jump with ingredient
        rb.simulated = false;
        float yOffset =0.5f;
        BoxCollider2D playerCollider = this.GetComponent<BoxCollider2D>();
        GameObject wholeGameObject = ingredientGameObject.transform.parent.gameObject;
        IngredientController ic = wholeGameObject.GetComponent<IngredientController>();
        ic.DisableProgressBar();
        wholeGameObject.transform.SetParent(this.gameObject.transform); // set the player game object as the parent of the ingredient
        if (!isFacingRight)
        {
            ingredientGameObject.transform.position = new Vector2(playerCollider.bounds.min.x - 0.75f,
                playerCollider.bounds.center.y + yOffset);
        }
        else
        {
            ingredientGameObject.transform.position = new Vector2(playerCollider.bounds.max.x+0.75f,
                playerCollider.bounds.center.y + yOffset);
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
            GameObject plate = this.gameObject.transform.GetChild(1).gameObject;
            Rigidbody2D rb = plate.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            hasPlate = false;
            plate.transform.SetParent(null);
        } 
        else if (isHoldingIngredient)
        {
            Debug.Log("Drop ingredient");
            Rigidbody2D rb = currentlyHoldingIngredient.GetComponent<Rigidbody2D>();
            currentlyHoldingIngredient.transform.localScale = new Vector3(1f, 1f, 1f);
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            
            rb.constraints = RigidbodyConstraints2D.None;
            currentlyHoldingIngredient.transform.parent.transform.SetParent(ingredientList.transform);

            isHoldingIngredient = false;
        }

       
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

    private void PlaceItemOnStove()
    {
        GameObject stoveGameObject = GameObject.FindGameObjectWithTag("Stove");
        BoxCollider2D stoveCollider = stoveGameObject.GetComponent<BoxCollider2D>();
        if (stoveGameObject.transform.childCount == 0 && isHoldingIngredient) // if there is nothing else on the stove
        {
            GameObject wholeGameObject = currentlyHoldingIngredient.transform.parent.gameObject;
            wholeGameObject.transform.SetParent(stoveGameObject.transform);
            wholeGameObject.transform.position = new Vector2(wholeGameObject.transform.position.x,
                wholeGameObject.transform.position.y + 5.0f); // move the item up a bit so it sits on the stove
            currentlyHoldingIngredient.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            IngredientController ic = wholeGameObject.GetComponent<IngredientController>();
            CookType cookType = ic.GetIngredientCookType();
            //Debug.Log("From Stove! "+ ic.currentIngredientState); 
            if (cookType == CookType.FIRE && (ic.currentIngredientState == IngredientCookingState.UNCOOKED || ic.currentIngredientState == IngredientCookingState.COOKING) )
            {
                ic.EnableProgressBar();
            }
            isHoldingIngredient = false;
        }
    }
    
    private void PlaceItemOnSink()
    {
        GameObject sinkGameObject = GameObject.FindGameObjectWithTag("Sink");
        if (sinkGameObject.transform.childCount == 0 && isHoldingIngredient)
        {
            GameObject wholeGameObject = currentlyHoldingIngredient.transform.parent.gameObject;
            wholeGameObject.transform.SetParent(sinkGameObject.transform);
            currentlyHoldingIngredient.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            wholeGameObject.transform.position = new Vector2(wholeGameObject.transform.position.x,
                wholeGameObject.transform.position.y + 5.0f);
            IngredientController ic = wholeGameObject.GetComponent<IngredientController>();
            CookType cookType = ic.GetIngredientCookType();
            if (cookType == CookType.WATER  && (ic.currentIngredientState == IngredientCookingState.UNCOOKED || ic.currentIngredientState == IngredientCookingState.COOKING))
            {
                ic.EnableProgressBar();
            }
            isHoldingIngredient = false;
        }
    }

    private void PickUpItemFromStove()
    {
        GameObject stoveGameObject = GameObject.FindGameObjectWithTag("Stove");
        if (stoveGameObject.transform.childCount == 1) // there is something on the stove so pick it up
        {
            GameObject wholeObject = stoveGameObject.transform.GetChild(0).gameObject;
            PlayerPickUpIngredient(wholeObject.transform.GetChild(0).gameObject);
        }
        isHoldingIngredient = true; 
    }

    private void PickUpItemFromSink()
    {
        GameObject sinkGameobject = GameObject.FindGameObjectWithTag("Sink");
        if (sinkGameobject.transform.childCount == 1) // there is something on the stove so pick it up
        {
            GameObject wholeObject = sinkGameobject.transform.GetChild(0).gameObject;
            PlayerPickUpIngredient(wholeObject.transform.GetChild(0).gameObject);
        }
        isHoldingIngredient = true;
    }

}
 