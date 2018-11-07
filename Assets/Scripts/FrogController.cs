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
            if (launchTime >= 110) // THis conditional should be replaced with whenever it hits the floor
            {
                isLaunching = false;
                launchTime = 0;
                frogCollider.sharedMaterial = stopMaterial;
                GetComponent<Rigidbody2D>().gravityScale = 1;
            }
            else
            {
                if(GetComponent<Rigidbody2D>().gravityScale < 1)
                {
                    GetComponent<Rigidbody2D>().gravityScale = GetComponent<Rigidbody2D>().gravityScale + 0.01f;
                }

                launchTime++;
            }
            if (launchTime<90)
            {
                int tempRotationZ = -4;
                if (facingRight)
                {
                    tempRotationZ = 4;
                }
                Vector3 tempVector = new Vector3(0, 0, tempRotationZ);
                transform.Rotate(tempVector);
            }
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
                if (Input.GetKeyUp("space") || Input.GetMouseButtonUp(0))  // Letting go of space while charging makes you stop charging
                {
                    // ================EXPERIMENTAL CODE: DANGER DANGER==============================
                    //GetComponent<Rigidbody2D>().velocity = fCrosshair.GetNormalizedAimVector()*launchSpeed;
                    GetComponent<Rigidbody2D>().velocity = new Vector2(fCrosshair.GetNormalizedAimVector().x * launchSpeed, fCrosshair.GetNormalizedAimVector().y * launchSpeed);
                    //=================================Nice==========================================
                    isCharging = false;
                    isLaunching = true;
                    frogCollider.sharedMaterial = launchMaterial;

                    GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                }
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
}
