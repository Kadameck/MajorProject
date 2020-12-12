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
    [SerializeField]
    Renderer SUAVisualisation;
    [SerializeField]
    GameObject magicBall;
    [SerializeField]
    Transform hand;

    [HideInInspector]
    public bool isClimbing = false;

    // Movement y value
    private float yDirect;

    // The rigidbody component of the player
    private Rigidbody rb;

    private bool canUsebasicMagic = true;
    private bool useMagic = false;
    private GameObject currentMagicBall;

    // Awake is called at the spawn of the object
    void Awake()
    {
        // Takes the rigidbody component of the player
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called regularly at a fixed interval
    void FixedUpdate()
    {
        UseMagic();
        Movement();
    }


    /// <summary>
    /// Controlls the player movement
    /// </summary>
    private void Movement()
    {
        // Checks if the normal Movement should be executed or if the player is climbing
        if (!isClimbing)
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
    /// Activates the Basic Magic Skill
    /// </summary>
    public void UseMagic()
    {
        // Checks if the player does a left click and the basic Magic skill doesn't have a cooldown currently
        if (Input.GetMouseButtonDown(0) && canUsebasicMagic)
        {
            // Checks whether the magic skill is already activated
            if (!useMagic)
            {
                // Makes the usable area visualization enabled
                StartCoroutine(ActivateSkillUseArea());

                // Spawns a magic ball effekt and set is a child of the hand of the player
                currentMagicBall = Instantiate(magicBall, hand.position, Quaternion.identity);
                currentMagicBall.transform.SetParent(hand);
                // Works against a Unity Bug that causes that Instantiate is called twice sometimes.
                CheckForSpawningBug();
            }
            else
            {
                Vector3 clickPos = Vector3.zero;
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {
                    clickPos = hit.point;

                    // Distance between the point in world space where the player has clicked on and the player itself
                    float dist = Vector3.Distance(transform.position, clickPos);
                    // Checks if the clicked point is in the magic usable area and if itwas a click on te terrain
                    if (dist <= 10 && (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("MagicInteractive")))
                    {
                        // Starts the basic magic ball behaviour
                        currentMagicBall.GetComponent<BasicMagicBehaviour>().Throw(transform.position, hit.point, dist);
                        currentMagicBall = null;

                        // Resets everything to no "magic not active"
                        useMagic = false;
                        SUAVisualisation.enabled = false;
                        canUsebasicMagic = false;

                        // Starts a magic usable cooldown
                        StartCoroutine(BasicMagicCooldown());
                    }
                }
            }
        }
        // Checks if the player is doing a rightclick during the basic magic skill is active
        else if (useMagic &&  Input.GetMouseButtonDown(1))
        {
            // Resets everything to no "magic not active"
            useMagic = false;
            SUAVisualisation.enabled = false;

            // Destry the unused magic ball
            Destroy(currentMagicBall);
        }
    }

    /// <summary>
    /// Determines if the player should sneak
    /// </summary>
    /// <returns>Should sneak</returns>
    public bool Sneak()
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
    public bool GetUseMagic()
    {
        return useMagic;
    }

    /// <summary>
    /// Makes the skill use Area visible at end of frame
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivateSkillUseArea()
    {
        yield return new WaitForEndOfFrame();
        useMagic = true;
        SUAVisualisation.enabled = true;
    }

    /// <summary>
    /// Gives the basic magic skill a cooldown time
    /// </summary>
    /// <returns></returns>
    IEnumerator BasicMagicCooldown()
    {
        yield return new WaitForSeconds(2);
        canUsebasicMagic = true;
    }

    /// <summary>
    /// Works against a Unity Bug that causes that Instantiate is called twice sometimes.
    /// Destroy all unnecessary Basic Magic Balls
    /// </summary>
    private void CheckForSpawningBug()
    {
        // Checks if there are more than one basic magic ball
        GameObject[] basicMagicBalls = GameObject.FindGameObjectsWithTag("BasicMagic");

        // id there is more than one ball...
        if(basicMagicBalls.Length > 1)
        {
            // Iterator for the balls
            int count = 0;

            // Loops through every ball
            foreach (GameObject ball in basicMagicBalls)
            {
                // Destroys one of the 2 (by the bug) spawned balls
                if (count == 0)
                { 
                    Destroy(ball);
                }
                count = 1;
            }
        }
    }
}
