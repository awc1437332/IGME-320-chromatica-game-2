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

    [SerializeField]
    public float score = 0;

    [SerializeField]
    public bool isActive = true;

    //This is the rigid body of the player
    Rigidbody rb;

    //Unity event for death
    public static UnityEvent PlayerDeath = new UnityEvent();

    [SerializeField]
    private GameObject progressBar;
    private FillProgressBar progressBarScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        progressBarScript = progressBar.GetComponent<FillProgressBar>();
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
                //velocity = jumpForce * Time.deltaTime;
                rb.AddForce(jumpForce, ForceMode.Impulse);

                isGrounded = false;
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
        }
    }

    /// <summary>
    /// This method is the collision detection and should be running on update automatically.
    /// I was having trouble getting it to run but this is the framework feel free to edit it.
    /// </summary>
    /// <param name="collision">This parameter is the collision that is being detected and passed through</param>
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Collectible")
        {
            //Debug.Log("You have collided with a collectible");

            // Increment progress bar.
            progressBarScript.IncrementCollectible();

            // Hide the GameObject, but do not deactivate it.
            //
            // Prevents interfering with Lanes.ClearLanes's bulk deactivation,
            // since references to pooled objects are maintained in each Lane's
            // underlying array before they get cleared.
            collision.gameObject.GetComponent<Renderer>().enabled = false;
            score++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
