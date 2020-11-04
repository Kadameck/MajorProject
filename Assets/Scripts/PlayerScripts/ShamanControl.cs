using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanControl : MonoBehaviour
{
    // The Speet multiplicator of the playeer
    [SerializeField]
    float walkSpeed = 500;
    [SerializeField]
    float rotationSpeed = 10;

    // The rigidbody component of the player
    private Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        // Takes the rigidbody component of the player
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            
            // Moves the player
            rb.velocity = new Vector3(xMove, rb.velocity.y, zMove) * walkSpeed * Time.deltaTime;

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
}
