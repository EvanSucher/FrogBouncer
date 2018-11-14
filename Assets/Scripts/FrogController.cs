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
    bool isCharging = false;
    bool isLaunching = false;
    int launchTime = 0;
    int chargeTime = 0;
    int timeBeforeLaunchDecceleration = 140;
    float deccel = 0.98f;
    float stopSpeed = 5;
    float RotationSpeed = 4;
    float gravIncreaseRate = 0.01f;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        frogCollider = GetComponent<CircleCollider2D>();
        frogCollider.sharedMaterial = stopMaterial;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float move = Input.GetAxis("Horizontal");

        CheckChargeInput();
        if (!isLaunching)
        {

            if (!isCharging) //not charging
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
                if (move > 0 && !facingRight)
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
                if (fCrosshair.isPointingRight() && !facingRight)
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
            if (launchTime >= timeBeforeLaunchDecceleration) // THis conditional should be replaced with whenever it hits the floor
            {
                GetComponent<Rigidbody2D>().gravityScale = 1;

                Vector2 tempVelocityVector = new Vector2(GetComponent<Rigidbody2D>().velocity.x * deccel, GetComponent<Rigidbody2D>().velocity.y * deccel);
                GetComponent<Rigidbody2D>().velocity = tempVelocityVector;
            }
            else
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
        if(!isLaunching)
        {
            if (!isCharging) // Not Charging
            {
                if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)) // Pressing Space while not charging makes you charge
                {
                    isCharging = true;
                }
            }
            else // charging
            {
                if (!Input.GetKey("space") && !Input.GetMouseButton(0))  // Letting go of space while charging makes you stop charging
                {
                    float charge = (10 + chargeTime) / 60f;
                    // ================EXPERIMENTAL CODE: DANGER DANGER==============================
                    //GetComponent<Rigidbody2D>().velocity = fCrosshair.GetNormalizedAimVector()*launchSpeed;
                    GetComponent<Rigidbody2D>().velocity = new Vector2(fCrosshair.GetNormalizedAimVector().x * (launchSpeed*charge), fCrosshair.GetNormalizedAimVector().y * (launchSpeed * charge));
                    //=================================Nice==========================================
                    isCharging = false;
                    isLaunching = true;
                    frogCollider.sharedMaterial = launchMaterial;
                    fCrosshair.transform.localScale = new Vector3(2, 2, fCrosshair.transform.localScale.z);

                    GetComponent<Rigidbody2D>().gravityScale = 0.5f + (charge/120);
                    chargeTime = 0;
                }
                if (Input.GetMouseButton(1))
                {
                    isCharging = false;
                    chargeTime = 0;
                    GetComponent<Rigidbody2D>().gravityScale = 1f;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                if (chargeTime < 50)
                {
                    chargeTime++;
                    float tempScale = 1 + (chargeTime / 50f) * 2;
                    fCrosshair.transform.localScale = new Vector3(tempScale, tempScale, fCrosshair.transform.localScale.z);
                    mCrosshair.transform.localScale = new Vector3(1+tempScale, 1+tempScale, fCrosshair.transform.localScale.z);
                }
            }
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        float currentVelocity = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) * 0.6f;
        RotationSpeed = currentVelocity - 3;
        Debug.Log(currentVelocity);
        if (isLaunching)
        {
            if (Input.GetKey("space") || Input.GetMouseButton(0))
            {
                if (other.contacts[0].normal == Vector2.up) //ground
                {
                    Debug.Log("ground");
                    StopLaunch();
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    isCharging = true;

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
            if (other.contacts[0].normal == Vector2.up && currentVelocity < stopSpeed) //if you hit the ground after a certain amount of time
            {
                Debug.Log("ground");
                StopLaunch();
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (other.contacts[0].normal == Vector2.right || other.contacts[0].normal == Vector2.left)
            {
                Flip();
            }
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void StopLaunch()
    {
        isLaunching = false;
        launchTime = 0;
        frogCollider.sharedMaterial = stopMaterial;
        GetComponent<Rigidbody2D>().gravityScale = 1;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    void RotateInAir()
    {
        float tempRotationZ = -RotationSpeed;
        if (facingRight)
        {
            tempRotationZ = RotationSpeed;
        }
        Vector3 tempRotationVector = new Vector3(0, 0, tempRotationZ);
        transform.Rotate(tempRotationVector);
    }
}
