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
    float moveSpeed = .01f;


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
    //These variables are used to have the player jump
    Vector3 playerPosition;
    Vector3 direction = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    //These fields are serailzed to make experimentation easier
    [SerializeField]
    Vector3 jumpForce;
    
    [SerializeField]
    Vector3 gravityForce = new Vector3(0.0f, -.0009f, 0.0f);

    [SerializeField]
    bool isGrounded = true;
    #endregion

    [SerializeField]
    public float score = 0;

    [SerializeField]
    public bool isActive = true;

    //Unity event for death
    public static UnityEvent PlayerDeath = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = transform.position;
        jumpForce = new Vector3(0.0f, .111f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //This method checks to see if the player is on the ground
            OnGround();

            //This code will run if the user presses space or w and the player is on the ground
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
            {
                velocity = jumpForce;
                isGrounded = false;
            }

            //This line will update the position vector
            playerPosition += velocity;

            //This line will change the position of the player
            transform.position = playerPosition;

            //If the player is in the air this will run to apply gravity
            if (!isGrounded)
            {
                velocity += gravityForce;
            }
            //This will run to stabilize the player at the ground level of the tiles
            else
            {
                velocity = Vector3.zero;
                playerPosition = new Vector3(playerPosition.x, 1.5f, playerPosition.z);
                transform.position = playerPosition;
            }


            //This will run if the input to move left is detected
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (lanePosition == activeLane.right)
                {
                    lanePosition = activeLane.middle;

                    playerPosition = new Vector3(0.0f, playerPosition.y, playerPosition.z);
                }
                else if (lanePosition == activeLane.middle)
                {
                    lanePosition = activeLane.left;

                    playerPosition = new Vector3(-3.33f, playerPosition.y, playerPosition.z);
                }
            }
            //This will run if the input to move right is detected
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (lanePosition == activeLane.left)
                {
                    lanePosition = activeLane.middle;

                    playerPosition = new Vector3(0, playerPosition.y, playerPosition.z);
                }
                else if (lanePosition == activeLane.middle)
                {
                    lanePosition = activeLane.right;

                    playerPosition = new Vector3(3.33f, playerPosition.y, playerPosition.z);
                }
            }
        }
    }

    /// <summary>
    /// This method is the collision detection and should be running on update automatically.
    /// I was having trouble getting it to run but this is the framework feel free to edit it.
    /// </summary>
    /// <param name="collision">This parameter is the collision that is being detected and passed through</param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Collectible")
        {
            //Debug.Log("You have collided with a collectible");
            Destroy(collision.gameObject);
            score++;
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            //Debug.Log("You have collided with an obstacle");
            TogglePlayer(false);
            PlayerDeath.Invoke();
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
            //Debug.Log("Player is on the ground");
        }
        else
        {
            isGrounded = false;
            //Debug.Log("Player is in the air");
        }
    }

    //Allows changing the state of the player between active and inactive
    private void TogglePlayer(bool value)
    {
        isActive = value;
    }
}
