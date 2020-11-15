using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Controler (Shaman)
/// </summary>
public class ShamanControl : MonoBehaviour
{
    // The Speed multiplicators of the playeer
    [SerializeField]
    float walkSpeed = 500;
    [SerializeField]
    float sneakSpeed = 150;
    [SerializeField]
    float rotationSpeed = 10;

    // Movement y value
    private float yDirect;

    // The rigidbody component of the player
    private Rigidbody rb;

    // Awake is called at the spawn of the object
    void Awake()
    {
        // Takes the rigidbody component of the player
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called regularly at a fixed interval
    void FixedUpdate()
    {
        // Calls the Movement functon
        Movement();
    }

    /// <summary>
    /// Controlls the player movement
    /// </summary>
    private void Movement()
    {
        // Checks if the player wants to move
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            // Takes the movement direktion 
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            // Takes the new movement Direction
            Vector3 targetDir = new Vector3(xMove, 0.0f, zMove).normalized;

            // Calls the PlayerRotation function
            PlayerRotation(targetDir);

            // Calls the GroundScann function to check whether there is ground under the player and, if so, in which y direction the player must move
            if (GroundScan())
            {
                // Checks if the player should sneak
                if (Sneak())
                {
                    // Moves the player in sneak speed
                    rb.velocity = new Vector3(xMove, yDirect, zMove) * sneakSpeed * Time.deltaTime;
                }
                else
                {
                    // Moves the player normal speed
                    rb.velocity = new Vector3(xMove, yDirect, zMove) * walkSpeed * Time.deltaTime;
                }
            }
            else
            {
                // Drops the player
                rb.velocity += new Vector3(0.0f, -1.0f, 0.0f);
            }
        }
    }

    /// <summary>
    /// Rotates the player in movement direction
    /// </summary>
    /// <param name="targetDir">The direction the player should look at</param>
    private void PlayerRotation(Vector3 targetDir)
    {
        // Prevents the player to rotate to 0,0,0 if there is noch input
        if(targetDir != Vector3.zero)
        {
            // Rotates the player smoothly so that it is looking to the movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Determines if the player should sneak
    /// </summary>
    /// <returns>Should sneak</returns>
    private bool Sneak()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Scans the area under the players position and determines if it is walkable ground
    /// </summary>
    /// <returns>walkability</returns>
    private bool GroundScan()
    {
        RaycastHit hit;

        // Checks if there is something within a distance of 1 below the player
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2.0f))
        {
            // Calculates the y direction value of the movement on the current ground
            yDirect = Vector3.Cross(hit.normal, transform.TransformDirection(Vector3.left)).y;
            return true;
        }

        return false;
    }
}
