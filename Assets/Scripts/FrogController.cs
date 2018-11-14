using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : MonoBehaviour {

    [SerializeField]
    float maxSpeed = 1;

    [SerializeField]
    float launchSpeed;

    [SerializeField]
    int launchRotation;

    [SerializeField]
    bool facingRight;

    [SerializeField]
    FrogCrosshair fCrosshair;

    [SerializeField]
    MouseCrosshair mCrosshair;

    [SerializeField]
    PhysicsMaterial2D launchMaterial;

    [SerializeField]
    PhysicsMaterial2D stopMaterial;

    [SerializeField]
    Animator anim;

    [SerializeField]
    CircleCollider2D frogCollider;


    private float closeCursorDistance = 0.3f;
    private bool isCharging = false;
    private bool isLaunching = false;
    private int launchTime = 0;
    private int chargeTime = 0;
    private int timeBeforeLaunchDecceleration = 200;
    private float deccel = 0.99f;
    private float stopSpeed = 5;
    private float RotationSpeed = 4;
    private float gravIncreaseRate = 0.01f;

    // Use this for initialization
    void Start ()
    {   // Initialize the animator, circle collider and circle collider material
        anim = GetComponent<Animator>();
        frogCollider = GetComponent<CircleCollider2D>();
        frogCollider.sharedMaterial = stopMaterial;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float move = Input.GetAxis("Horizontal");

        CheckChargeInput(); // Check's if the player is charging in order to update isCharging and isLaunching

        if (!isLaunching) // Not Launching
        {
            if (!isCharging) // Not Charging or Launching, basically when the player is idle
            {
                // This line is let's the player walk left and right while not charging or launching
                GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

                if (move > 0 && !facingRight) // Flips the player depending on direction
                {
                    Flip();
                }
                else if (move < 0 && facingRight)
                {
                    Flip();
                }
            }
            else // is charging
            {
                if (fCrosshair.isPointingRight() && !facingRight) // Flips Crosshair based on mouse location (SHOULD BE CHANGED DEPENDING ON ANGLE)
                {
                    Flip();
                }
                else if (!fCrosshair.isPointingRight() && facingRight)
                {
                    Flip();
                }
            }
        }
        else
        {
            if (launchTime >= timeBeforeLaunchDecceleration) // After the player has been launching for a given time
            {
                GetComponent<Rigidbody2D>().gravityScale = 1; //sets gravity to 1 (should already be 1 by this point but just in case)

                Vector2 tempVelocityVector = new Vector2(GetComponent<Rigidbody2D>().velocity.x * deccel, GetComponent<Rigidbody2D>().velocity.y * deccel);
                GetComponent<Rigidbody2D>().velocity = tempVelocityVector; // Slowly deccelerates the player
            }
            else // When the player is launching but before a set time, the gravity slowly increments to get greater over time
            {
                if(GetComponent<Rigidbody2D>().gravityScale < 1)
                {
                    GetComponent<Rigidbody2D>().gravityScale = GetComponent<Rigidbody2D>().gravityScale + gravIncreaseRate;
                }

                launchTime++;
            }

            RotateInAir();

        }

        anim.SetFloat("speed", (Mathf.Abs(move) + 0.5f)*2);
        anim.SetBool("isCharging", isCharging);
        anim.SetBool("isLaunching", isLaunching);
    }

    public void CheckChargeInput()
    {
        if(!isLaunching) //If the  player is Launching, then don't do anything
        {
            if (!isCharging) // Not Charging
            {
                if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)) // Pressing Space/mouse1 while not charging makes you charge
                {
                    isCharging = true;
                }
            }
            else // charging
            {
                if (!Input.GetKey("space") && !Input.GetMouseButton(0))  // Letting go of space/mouse1 while charging makes you stop charging
                {
                    float charge = (10 + chargeTime) / 60f; // charge is the value that 

                    // ================ EXPERIMENTAL CODE: DANGER DANGER (not really) ======================
                    // This basically just changes the velocity to the normal vector of the crosshair * the launchspeed * charge
                    GetComponent<Rigidbody2D>().velocity = new Vector2(fCrosshair.GetNormalizedAimVector().x * (launchSpeed*charge), fCrosshair.GetNormalizedAimVector().y * (launchSpeed * charge));
                    // =================================Nice================================================

                    isCharging = false; // Going from charging to launching
                    isLaunching = true;
                    frogCollider.sharedMaterial = launchMaterial;
                    fCrosshair.transform.localScale = new Vector3(2, 2, fCrosshair.transform.localScale.z);

                    RotationSpeed = 10;

                    GetComponent<Rigidbody2D>().gravityScale = 0.5f + (charge/120);
                    chargeTime = 0;
                }
                if (Input.GetMouseButton(1))  // Pressing the right mouse button will stop the charging and make the player go back to idle
                {
                    isCharging = false;
                    chargeTime = 0;
                    GetComponent<Rigidbody2D>().gravityScale = 1f;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                if (chargeTime < 50) // Increases charge time while the player is charging, and increase UI SIZE with charge
                {
                    chargeTime++;
                    float tempScale = 1 + (chargeTime / 50f) * 2; // Scale for the Crosshairs
                    fCrosshair.transform.localScale = new Vector3(tempScale, tempScale, fCrosshair.transform.localScale.z);
                    mCrosshair.transform.localScale = new Vector3(1+tempScale, 1+tempScale, fCrosshair.transform.localScale.z);
                }
            }
        }

    }

    void OnCollisionEnter2D(Collision2D other) // function called when there's a collision with the wall
    {   
        float currentVelocity = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) * 0.6f;
        RotationSpeed = currentVelocity - 3; // Thsese lines take the velocity of the player, and set the player rotation speed to match velocity

        if (isLaunching) // None of the code in here will run unless the player is launching
        {
            if (Input.GetKey("space") || Input.GetMouseButton(0)) // If the player is holding either space or 
            {
                // The if statements below, are basically designed to get the contact points of whatever the player collides with
                // and then finds the normal vector and compares it to Vector2.Up, Vector2.Left, etc. So far there is only up, down,
                // left, and right but the code could probably just be changed so there are no if statements, and there is just code
                // that turns the normal vector into an Euler angle, and set that as the transform.rotation
                if (other.contacts[0].normal == Vector2.up) //ground
                {
                    Debug.Log("ground");
                    StopLaunch(); // Stops the Launch
                    transform.rotation = Quaternion.Euler(0, 0, 0); // Set the Rotation
                    GetComponent<Rigidbody2D>().gravityScale = 0; //Change Gravity to 0
                    isCharging = true; //Immediately begin charging again

                }
                else if (other.contacts[0].normal == Vector2.left) //right wall
                {
                    Debug.Log("right wall");
                    StopLaunch();
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    isCharging = true;
                }
                else if (other.contacts[0].normal == Vector2.right) //left wall
                {
                    Debug.Log("left wall");
                    StopLaunch();
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    isCharging = true;
                }
                else if (other.contacts[0].normal == Vector2.down) //ceiling
                {
                    Debug.Log("ceiling");
                    StopLaunch();
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    isCharging = true;
                }
            }
            if (other.contacts[0].normal == Vector2.up && currentVelocity < stopSpeed) //if the player is slow enough and hits the ground
            {
                Debug.Log("ground");
                StopLaunch(); // returns the player to idle
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (other.contacts[0].normal == Vector2.right || other.contacts[0].normal == Vector2.left) // Only flips the player if hits a wall
            {
                Flip();
            }
        }

    }

    void Flip() //just flips the player
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void StopLaunch() // function to stop the player from launching
    {
        isLaunching = false;
        launchTime = 0; // reset launch timer
        frogCollider.sharedMaterial = stopMaterial; // changes material so the player does not bounce
        GetComponent<Rigidbody2D>().gravityScale = 1; // sets gravity to normal
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // sets velocity to normal
    }

    void RotateInAir()
    {
        float tempRotationZ = -RotationSpeed; // temporary rotation float 
        if (facingRight) 
        {
            tempRotationZ = RotationSpeed; // temporary rotation float is opposite if facing right
        }
        Vector3 tempRotationVector = new Vector3(0, 0, tempRotationZ); // vector3 to prime rotation
        transform.Rotate(tempRotationVector); // Rotates the player
    }
}
