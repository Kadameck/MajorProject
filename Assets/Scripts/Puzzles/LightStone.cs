using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The light stones the player can activate through basic magic
/// </summary>
public class LightStone : MonoBehaviour
{
    [SerializeField, Tooltip("Waht should be the start state of this light stone?")]
    bool isActive;
    [SerializeField, Tooltip("When activ, the light beam will no longer change during runtime. Only useful if neither the light source nor the target move or rotate in any way")]
    bool dynamicLightBeam = false;
    [SerializeField, Range(1, 20), Tooltip("At what distance should the stone start to emit light based on the distance of the player (in the deactivated state)")]
    int lightEffektRange = 20;
    [Space (10)]
    [SerializeField, Tooltip("Set here the start transfom of the lightbeam \n If you leave this field empty, the position of the object that has this script attached will be used")]
    Transform beamStart;
    [SerializeField, Tooltip("Set here the target transfom of the lightbeam \n If you leave this field empty, the lightbeam will go forwards")]
    Transform beamTarget;
    [SerializeField, Tooltip("Material used for the lightBeam")]
    Material beamMaterial;
    [Space (10)]
    [SerializeField, Tooltip("Color used near the startpoint")]
    Color startColor;
    [SerializeField, Tooltip("Color used near the targetpoint")]
    Color endColor;
    [SerializeField, Range(0,1), Tooltip("Alpha Value from 0 means 'invisible lightbeam'")]
    float alpha = 1.0f;
    [Space (10)]
    [SerializeField, Tooltip("The width of the lightbeam at every position of its lengh")]
    AnimationCurve widthCurve;

    // LineRenderer Component of this Object
    private LineRenderer lRend;
    // Color gradiant of the lightbeam
    private Gradient gradient;
    // Default initialized stat
    private bool isInitialized = false;
    // Material used by this Object
    private Material mat;
    // default emission value
    private float emission = -1;
    // Player Object
    private GameObject player;

    private GameObject currentTargetPrism;

    // Start is called before the first frame update
    void Start()
    {
        // Sets its own position as default start
        if(beamStart == null)
        {
            beamStart = this.gameObject.transform;
        }

        // Add and get the Linerenderer Component of the Lightstone and sets it to disabled fpr the start
        this.gameObject.AddComponent<LineRenderer>();
        lRend = GetComponent<LineRenderer>();
        lRend.enabled = false;
        
        // takes the light stones material and setzts its emission value to the given start value
        mat = this.gameObject.GetComponent<Renderer>().materials[0];
        mat.SetColor("_EmissionColor", Color.blue * emission);
        
        // Gets the player Object
        player = GameObject.FindGameObjectWithTag("Player");

        // Checks if the light stone should start active
        if (isActive)
        {
            StartCoroutine(WaitForInit());
        }
    }

    /// <summary>
    /// Counteracts the fact that the awake method of the prisms is only carried out after the start method of the light stones
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForInit()
    {
        // Ensures that the following code is only executed after the prisms awake method
        yield return new WaitForEndOfFrame();
        // Initialize the lineRenderer and sets its start and target point
        InitializeLine();
        //RaycastShot();
        FindTarget();

    }

    /// <summary>
    /// Calls nessesery Funktions each frame
    /// </summary>
    private void Update()
    {
        // Checks if the light stone is deactivated
        if (!isActive)
        {
            // Calls the getPlayerDistance Funktion
            GetPlayerDistance();
        }
        // Checks if the light stone is active but not emitted enough light
        else if (isActive && emission != 10)
        {
            // Sets the light emission to its highest value
            MaterialShininess(0);
        }

        // If the light stone starts inactive, the light stone have to be initialized by activate it through the player at the first time
        if(lRend.enabled && !isInitialized)
        {
            InitializeLine();
            //RaycastShot();
            FindTarget();
        }

        // Upgrades the target position of the line if it is nessesery
        if(dynamicLightBeam && lRend.enabled)
        {
            UpgradeStartPosition();
            //  RaycastShot();
            FindTarget();
        }
    }

    /// <summary>
    /// Initialize the first use of the lineRenderer
    /// </summary>
    private void InitializeLine()
    {
        // sets all needed values for the line
        lRend.widthCurve = widthCurve;
        lRend.positionCount = 2;
        lRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lRend.material = new Material(beamMaterial);

        if (beamStart != null)
        {
            lRend.SetPosition(0, beamStart.position);
        }

        if(beamTarget != null)
        {
            transform.LookAt(beamTarget.position);
        }
        // Sets the color of the line (start and target point)
        gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
                         new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) });
        lRend.colorGradient = gradient;
        lRend.enabled = true;
        // Sets the linerenderer as initialized so that this funktion is just called once
        isInitialized = true;
    }

    /// <summary>
    /// Handles activity state of the line renderer
    /// </summary>
    public void ChangeLineActiveState()
    {
        // Change the lineRenderer active state
        lRend.enabled = !lRend.enabled;
        // Sets the isActive bool as the same value as the lineRenderer enabled state
        isActive = lRend.enabled;

        if(!isActive)
        {
            FindTarget();
        }
    }

    /// <summary>
    /// Controlls the emission value of the material
    /// </summary>
    /// <param name="dist">x-axis value of the lightEmissionByPlayerDistance-Curve</param>
    private void MaterialShininess(float dist)
    {
        if(isActive)
        {
            emission = 1;
        }
        else
        {
            float a = (float) -1;
            float b = (float) lightEffektRange - dist;
            float c = (float) 1 / lightEffektRange;

            // Takes the y-axis value from the curve at the position of the given x-axis value
            // Mappt den bereich 20 (der bereich in dem sich die annäherung anpassen soll) auf den bereich -1 bis 0 (der bereich wie sich die emission ändern soll)
            emission = a + (b * c);
        }
        // Sets the material emission value
        mat.SetColor("_EmissionColor", Color.blue * emission);
    }

    /// <summary>
    /// Gets the distance between the player and the light stone
    /// </summary>
    private void GetPlayerDistance()
    {
        // gets the distance
        float dist = Vector3.Distance(transform.position, player.transform.position);
   
        // Checks whether the distance is less than the y-axis value of the specified curve
       // if (dist <= lightEmissionByPlayerDistance.keys[lightEmissionByPlayerDistance.keys.Length-1].time)
        if(dist<=lightEffektRange)
        {
            // Calls the MaterialShininess funktion with the current distance
            MaterialShininess(dist);
        }
        // If the players is to far but the light stone is still glowing...
        else if(dist > lightEffektRange && emission != -1)
      {
            // Calls the MaterialShininess Funktion with the y-axis value of the last Key in the curve
            MaterialShininess(lightEffektRange);// lightEmissionByPlayerDistance.keys[lightEmissionByPlayerDistance.keys.Length - 1].time);
      }
    }

    /// <summary>
    /// Tracks any movement of the light source
    /// </summary>
    private void UpgradeStartPosition()
    {
        // If there is no startpoint given, this object use it self
        if(beamStart == null)
        {
            lRend.SetPosition(0, transform.position);
        }
        // If there is a startpoint given the script will use its coordinates
        else
        {
            lRend.SetPosition(0, beamStart.position);
        }
    }


    /// <summary>
    /// Finds the endpoint of the LineRenderer line
    /// </summary>
    private void FindTarget()
    {
        RaycastHit hit;
        // Shoots a raycast in the forward direction
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50000))
        {
            // Sets the hitpoint as targetpoint of the lightbeam
            lRend.SetPosition(1, hit.point);

            // Checks if the lightbeam hits a prism
            if(hit.collider.gameObject.CompareTag("Prisma"))
            {
                // Checks if the lighstone is deaktive
                if(!isActive)
                {
                    // Checks if there is a currentTargetPrism
                    if(currentTargetPrism != null)
                    {
                        // Deactivates the currentTarget Prism (Be aware of the next codeline)
                        currentTargetPrism.GetComponent<Prism>().SetPrismDeactive();
                        // "currentTargetPrism.GetComponent<Prism>().SetPrismactive();" will not work correctly if the currentTargetPrism variable will be cleard without a timer
                        StartCoroutine(TimerAgainstThePrismDeactivationBug());
                    }
                }

                // Checks if the hitted object isnt the same prism as before
                if (hit.collider.gameObject != currentTargetPrism)
                {
                    // If there is currently no prism activated by this lightstone
                    if(currentTargetPrism != null)
                    {
                        // Deaktivates the currentTargetPrism
                        currentTargetPrism.GetComponent<Prism>().SetPrismDeactive();
                    }
                    
                    // Sets the new hitten prism as currently from this lightstone activated and actually activates it
                    currentTargetPrism = hit.collider.gameObject;
                    hit.collider.gameObject.GetComponent<Prism>().SetPrismActive(lRend, dynamicLightBeam);
                }
            }
            // IF the Raycast doesnt hit a prism
            else
            {
                // Checks if there is still a prim activ because of this lightstone
                if(currentTargetPrism != null)
                {
                    // deaktivates the currently active lightstone
                    currentTargetPrism.GetComponent<Prism>().SetPrismDeactive();
                    // Clears the currentTargetPrism variable back to null
                    currentTargetPrism = null;
                }
            }
        }
    }

    /// <summary>
    /// This timer is required because otherwise the current prism will not be deactivated, 
    /// even if the code line ,executed after the timer has expired, is inserted at the point at which this timer is called in the code
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerAgainstThePrismDeactivationBug()
    {
        yield return new WaitForEndOfFrame();
        // Resets the currentTargetPrisma Variable
        currentTargetPrism = null;
    }
}
