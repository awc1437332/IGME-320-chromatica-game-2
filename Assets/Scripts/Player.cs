using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    #region Variables for moving left and right
    //These variables are used to move the player between lanes
    const float height = 1.5f;
    const float left = -3.33f;
    const float middle = 0;
    const float right = 3.33f;
    
    //This is to be utilized later
    //float moveSpeed = .01f;


    //This enum is used to determine which lane the player is located in
    private enum activeLane
    {
        left,
        middle,
        right
    }

    //This variable starts the player in the middle lane
    activeLane lanePosition = activeLane.middle;
    #endregion 

    #region Variables for jumping

    //These fields are serailzed to make experimentation easier
    [SerializeField]
    Vector3 jumpForce;

    [SerializeField]
    bool isGrounded = true;
    #endregion

    public float Score { get; private set; }

    [SerializeField]
    public bool isActive = false;

    [SerializeField]
    public bool hasDoubleJump = false;
    private bool canDoubleJump = false;
    public float doubleJumpTimer;
    private const float doubleJumpTime = 20.0f;

    [SerializeField]
    public bool hasExtraLife = false;
    public bool isInvincible = false;
    public float invincibilityTimer;
    private const float invincibilityTime = 1.0f;

    [SerializeField]
    public bool hasDoubleCollectibles = false;
    public float doubleCollectibleTimer;
    private const float doubleCollectibleTime = 20.0f;

    //This is the rigid body of the player
    Rigidbody rb;

    //Unity event for death
    public static UnityEvent PlayerDeath = new UnityEvent();

    [SerializeField]
    private GameObject progressBar;
    private FillProgressBar progressBarScript;

    /// <summary>
    /// Value to increment score by each frame.
    /// </summary>
    [SerializeField]
    private float incrementScoreFactor;

    /// <summary>
    /// Rate at which to scale score increments each time the player levels up.
    /// </summary>
    [SerializeField]
    private float scoreIncrementScaleFactor;

    public int Level { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        progressBarScript = progressBar.GetComponent<FillProgressBar>();
        Score = 0;
        Level = 1;

        // Increment level when progress bar is filled.
        FillProgressBar.levelUp.AddListener(LevelUp);
        FillProgressBar.levelUp.AddListener(ScaleIncrement);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //This method checks to see if the player is on the ground
            OnGround();
            
            //This code will run if the user presses space or w and the player is on the ground
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)))
            {
                //Regular Jump
                if (isGrounded)
                {
                    //velocity = jumpForce * Time.deltaTime;
                    rb.AddForce(jumpForce, ForceMode.Impulse);

                    isGrounded = false;
                }
                //Double Jump
                else if (!isGrounded && (hasDoubleJump && canDoubleJump))
                {
                    //velocity = jumpForce * Time.deltaTime;
                    rb.AddForce(jumpForce, ForceMode.Impulse);

                    canDoubleJump = false;
                }
            }

            //This will run if the input to move left is detected
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (lanePosition == activeLane.right)
                {
                    lanePosition = activeLane.middle;

                    transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
                }
                else if (lanePosition == activeLane.middle)
                {
                    lanePosition = activeLane.left;

                    transform.position = new Vector3(-3.33f, transform.position.y, transform.position.z);
                }
            }
            //This will run if the input to move right is detected
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (lanePosition == activeLane.left)
                {
                    lanePosition = activeLane.middle;

                    transform.position = new Vector3(0, transform.position.y, transform.position.z);
                }
                else if (lanePosition == activeLane.middle)
                {
                    lanePosition = activeLane.right;

                    transform.position = new Vector3(3.33f, transform.position.y, transform.position.z);
                }
            }

            Score += incrementScoreFactor;

            //If the player has the double jump powerup, 
            if (hasDoubleJump)
            {
                //Reduce the timer per frame
                doubleJumpTimer -= Time.deltaTime;

                //If time runs out,
                if (doubleJumpTimer <= 0)
                {
                    //Remove the double jump powerup
                    hasDoubleJump = false;
                }
            }

            //If the player has the double collectible powerup, 
            if (hasDoubleCollectibles)
            {
                //Reduce the timer per frame
                doubleCollectibleTimer -= Time.deltaTime;

                //If time runs out,
                if (doubleCollectibleTimer <= 0)
                {
                    //Remove the double collectible powerup
                    hasDoubleCollectibles = false;
                }
            }
        }
        else if (isInvincible)
        {
            //Reduce the timer per frame
            invincibilityTimer -= Time.deltaTime;

            //If time runs out,
            if (invincibilityTimer <= 0)
            {
                //Remove the invincibility and give control back to the player
                isInvincible = false;
                TogglePlayer(true);
            }
        }
    }

    /// <summary>
    /// This method is the collision detection and should be running on update automatically.
    /// I was having trouble getting it to run but this is the framework feel free to edit it.
    /// </summary>
    /// <param name="collision">This parameter is the collision that is being detected and passed through</param>
    void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Collectible":
                //Debug.Log("You have collided with a collectible");

                // Increment progress bar.
                progressBarScript.IncrementCollectible();

                // Hide the GameObject, but do not deactivate it.
                //
                // Prevents interfering with Lanes.ClearLanes's bulk deactivation,
                // since references to pooled objects are maintained in each Lane's
                // underlying array before they get cleared.
                collision.gameObject.GetComponent<Renderer>().enabled = false;
                Score += (Convert.ToInt32(hasDoubleCollectibles) + 1) * (int)(Level * Level);
                //Debug.Log(Score);
                break;
            case "JumpTrigger":
                // Increment progress bar.
                progressBarScript.IncrementJump();
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Does not check collision if the player is invincible
        if (!isInvincible)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                //Debug.Log("You have collided with an obstacle");

                //Consumes the player's extra life if they have one and start the invincibility timer
                if (hasExtraLife)
                {
                    hasExtraLife = false;
                    isInvincible = true;
                    TogglePlayer(false);
                    return;
                }

                TogglePlayer(false);
                PlayerDeath.Invoke();
            }
            else if (collision.gameObject.tag == "Shop")
            {
                collision.gameObject.GetComponent<Shop>().ToggleShop(true);

                TogglePlayer(false);
            }
        }
    }

    /// <summary>
    /// This is a private helper method that determines if the player is on the ground outside of the jump calculation
    /// </summary>
    private void OnGround()
    {
        if(transform.position.y <= 1.5)
        {
            isGrounded = true;
            canDoubleJump = true;
            //Debug.Log("Player is on the ground");
        }
        else
        {
            isGrounded = false;
            //Debug.Log("Player is in the air");
        }
    }

    //Allows changing the state of the player between active and inactive
    public void TogglePlayer(bool value)
    {
        //Prevents the player from moving
        isActive = value;
        rb.isKinematic = !value;

        //Disables the player's animation
        GetComponent<Animator>().enabled = value;
    }

    //Gives the player a double jump from the shop
    public void ActivateDoubleJump(int price)
    {
        //Pays the cost and gives the player double jump
        Score -= price;
        hasDoubleJump = true;
        doubleJumpTimer = doubleJumpTime;
    }

    //Gives the player an extra life from the shop
    public void ActivateExtraLife(int price)
    {
        //Pays the cost and gives the player an extra life
        Score -= price;
        hasExtraLife = true;
        invincibilityTimer = invincibilityTime;
    }

    //Gives the player a double collectibles boost from the shop
    public void ActivateDoubleCollectibles(int price)
    {
        //Pays the cost and gives the player double jump
        Score -= price;
        hasDoubleCollectibles = true;
        doubleCollectibleTimer = doubleCollectibleTime;
    }

    /// <summary>
    /// Increments player level.
    /// </summary>
    private void LevelUp()
    {
        Level++;
    }

    private void ScaleIncrement()
    {
        incrementScoreFactor *= scoreIncrementScaleFactor;
    }
}
