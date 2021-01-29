using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Controler (Shaman)
/// </summary>
public class ShamanControl : MonoBehaviour
{
    [SerializeField]
    CameraBehaviour mainCamera;
    [SerializeField, Tooltip("Character Animator Component")]
    Animator anim;
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
    [SerializeField]
    GameObject carrySphere;
    [SerializeField, Tooltip("The Speed multiplicators of the playeer")]
    public float walkSpeed = 500;

    [HideInInspector]
    public bool isClimbing = false;
    [HideInInspector]
    public bool isSneaking = false;


    [Space(15)]
    [SerializeField]
    audioCollector[] soundeffects;

    // Movement y value
    private float yDirect;
    // The rigidbody component of the player
    private Rigidbody rb;
    private bool canUsebasicMagic = true;
    private bool useMagic = false;
    private bool puttDownSomething = false;
    private GameObject currentMagicBall;
    private GameObject currentlyCarriedObject;
    private bool pushingSomething = false;
    private bool grounded;
    private bool controlable = false;

    IEnumerator lGC = null;

    // Awake is called at the spawn of the object
    void Awake()
    {
        // Takes the rigidbody component of the player
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SoundManager.InitAudio(soundeffects);
    }

    // FixedUpdate is called regularly at a fixed interval
    void FixedUpdate()
    {
        if(isSneaking && isClimbing)
        {
            isSneaking = false;
            anim.SetBool("Sneak", false);
        }

        if (controlable)
        {
            GroundScan();
            Movement();
            Interact();
        }
    }

    public void MakeControlable()
    {
        mainCamera.SetPlayerControlableTrue();
        controlable = true;
    }

    /// <summary>
    /// Controlls the player movement
    /// </summary>
    private void Movement()
    {
        // Checks if the normal Movement should be executed or if the player is climbing
        if (!isClimbing)
        {
            if(grounded) //GroundScan())
            {
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    // Takes the movement direktion 
                    float xMove = Input.GetAxisRaw("Horizontal");
                    float zMove = Input.GetAxisRaw("Vertical");

                    // Takes the new movement Direction
                    Vector3 targetDir = new Vector3(xMove, 0.0f, zMove).normalized;

                    // Calls the PlayerRotation function
                    PlayerRotation(targetDir);

                    if (Input.GetKeyDown(KeyCode.LeftControl))
                    {
                        if (currentMagicBall != null)
                        {
                            SUAVisualisation.enabled = false;
                            useMagic = false;
                            Destroy(currentMagicBall);
                            currentMagicBall = null;
                            anim.SetBool("Magic", false);
                        }
                        Sneak();
                    }

                    // Checks if the player should sneak
                    if (isSneaking || pushingSomething)
                    {
                        Debug.Log("blablabla");
                        anim.SetBool("Walk", false);
                        anim.SetBool("Sneak", true);
                        // Moves the player in sneak speed
                        rb.velocity = new Vector3(xMove, yDirect, zMove) * sneakSpeed * Time.deltaTime;
                    }
                    else
                    {
                        Debug.Log("teststst");
                        // Moves the player normal speed
                        rb.velocity = new Vector3(xMove, yDirect, zMove) * walkSpeed * Time.deltaTime;
                        anim.SetBool("Sneak", false);
                        anim.SetBool("Walk", true);
                        SoundManager.PlaySound(SoundManager.Sound.Walk);
                    }
                    // Drops the player
                    rb.velocity += new Vector3(0.0f, -1.0f, 0.0f);
                }
                else
                {
                    anim.SetBool("Walk", false);
                }
            }
            else
            {
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
    /// Check for Interaction
    /// </summary>
    private void Interact()
    {
        // Try to Carry something
        Carry();

        // If the player doesnt carry anysthing
        if(currentlyCarriedObject == null)
        {
            // Chekcs if the player was put down something this frame
            if(puttDownSomething)
            {
                // Start a timer to prevent the Basic Magic Skill from being activated from the same click action the player use to put down the object that her currently carried
                StartCoroutine(PutDownResetTimer());
            }
            else
            {
                // Activated the basic magic skill
                UseMagic();
            }
        }
    }

    /// <summary>
    /// Start and stop the carring ob an portable object
    /// </summary>
    private void Carry()
    {
        if (Input.GetMouseButtonDown(0) && !useMagic)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Checks if it is a portable object the player clicked on
                if (hit.collider.gameObject.GetComponent<CarryObject>() != null)
                {
                    // Checks if the player doesn't is carrying something right now
                    if (currentlyCarriedObject == null)
                    {
                        currentlyCarriedObject = hit.collider.gameObject;
                        currentlyCarriedObject.GetComponent<CarryObject>().Take(carrySphere);

                        // Set the clicked object as carried object
                    }
                }
                // If the cklicked object was not a portable object
                else
                {
                    // if the player is carring a object right now...
                    if (currentlyCarriedObject != null)
                    {
                        // Checks if the clicked spot is a valide one to placing something
                        if (hit.collider.gameObject.CompareTag("Ground"))
                        {
                            try
                            {
                                // Put the carried object down and resets all carring variables
                                currentlyCarriedObject.GetComponent<CarryObject>().PutDown(hit.point);
                                currentlyCarriedObject = null;
                                puttDownSomething = true;
                            }
                            catch
                            {
                                currentlyCarriedObject = null;
                                puttDownSomething = true;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Activates the Basic Magic Skill
    /// </summary>
    public void UseMagic()
    {
        if((Input.GetMouseButtonDown(1) && useMagic) || GetPushSomething() || isClimbing)
        {
            SUAVisualisation.enabled = false;
            useMagic = false;
            Destroy(currentMagicBall);
            currentMagicBall = null;
            anim.SetBool("Magic", false);
        }

        // Checks if the player does a left click and the basic Magic skill doesn't have a cooldown currently
        if (Input.GetMouseButtonDown(0) && canUsebasicMagic)
        {
            // Checks whether the magic skill is already activated
            if (!useMagic)
            {
                // Makes the usable area visualization enabled
                StartCoroutine(ActivateSkillUseArea());
                
                if(isSneaking)
                {
                    Sneak();
                    anim.SetBool("Sneak", false);
                }
                
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
                        anim.SetBool("Magic", false);

                        // Starts a magic usable cooldown
                        StartCoroutine(BasicMagicCooldown());
                    }
                }
            }
        }
        // Checks if the player is doing a rightclick during the basic magic skill is active
        else if (useMagic &&  Input.GetMouseButtonDown(1) || GetPushSomething() || isClimbing)
        {
            // Resets everything to no "magic not active"
            useMagic = false;
            SUAVisualisation.enabled = false;
            anim.SetBool("Magic", false);
            // Destry the unused magic ball
            Destroy(currentMagicBall);
        }
    }

    /// <summary>
    /// Determines if the player should sneak
    /// </summary>
    /// <returns>Should sneak</returns>
    public void Sneak()
    {
        isSneaking = !isSneaking;
    }

    /// <summary>
    /// Scans the area under the players position and determines if it is walkable ground
    /// </summary>
    /// <returns>walkability</returns>
    private void GroundScan()
    {
        RaycastHit hit;

        // Checks if there is something within a distance of 1 below the player
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2.0f))
        {
            // Calculates the y direction value of the movement on the current ground
            yDirect = Vector3.Cross(hit.normal, transform.TransformDirection(Vector3.left)).y;

            if(lGC != null)
            {
                StopCoroutine(lGC);
                lGC = null;
            }

            grounded = true;
            //return true;
        }

        if(lGC == null)
        {
            lGC = LostGroundContact();
            StartCoroutine(lGC);
        }
        //return false;
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
        anim.SetBool("Magic", true);
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

    // Start a timer to prevent the Basic Magic Skill from being activated from the same click action the player use to put down the object that her currently carried
    IEnumerator PutDownResetTimer()
    {
        yield return new WaitForEndOfFrame();
        puttDownSomething = false;
    }

    public bool GetPushSomething()
    {
        return pushingSomething;
    }

    public void SetPushingSomething()
    {
        pushingSomething = !pushingSomething;

        if(pushingSomething)
        {
            anim.SetBool("Push", true);
            anim.SetFloat("StartEndPush", 2.0f);
        }
        else
        {
            anim.SetBool("Push", false);
            anim.SetFloat("StartEndPush", -2.0f);
        }
    }

    public bool GetIsGrounded()
    {
        return grounded;
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public bool getUseMagic()
    {
        return useMagic;
    }

    private IEnumerator LostGroundContact()
    {
        yield return new WaitForSeconds(0.2f);
        grounded = false;
    }
}
