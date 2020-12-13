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
    [SerializeField, Tooltip("Should the line update when the start or end point moves?")]
    bool trackRotation = false;   
    [SerializeField, Tooltip("Set here the start transfom of the lightbeam \n If you leave this field empty, the position of the object that this script attached will be used")]
    Transform beamStart;
    [SerializeField, Tooltip("Material used for the lightBeam")]
    Material beamMaterial;
    [SerializeField, Tooltip("Color used near the startpoint")]
    Color startColor;
    [SerializeField, Tooltip("Color used near the targetpoint")]
    Color endColor;
    [SerializeField, Range(0,1), Tooltip("Alpha Value from 0 means 'invisible lightbeam'")]
    float alpha = 1.0f;
    [SerializeField, Tooltip("The width of the lightbeam at every position of its lengh")]
    AnimationCurve widthCurve;
    [SerializeField, Tooltip("The light emission value based on the players distance \n x-Axis: Emission value \n y-Axis: Player distance")]
    AnimationCurve lightEmissionByPlayerDistance;

    // LineRenderer Component of this Object
    private LineRenderer lRend;
    // Color gradiant of the lightbeam
    private Gradient gradient;
    // Default initialized stat
    private bool isInitialized = false;
    // Material used by this Object
    private Material mat;
    // default emission value
    private float emission = 10;
    // Player Object
    private GameObject player;

    private GameObject currentTargetPrisma;

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
            // Initialize the lineRenderer and sets its start and target point
            InitializeLine();
            //UpgradeStartAndTarget();
            RaycastShot();
        }
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
            RaycastShot();
        }

        // Upgrades the start and/or target position of the line if it is nessesery
        if(trackRotation && lRend.enabled)
        {
            RaycastShot();
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

        lRend.SetPosition(0, beamStart.position);

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
    }

    /// <summary>
    /// Controlls the emission value of the material
    /// </summary>
    /// <param name="dist">x-axis value of the lightEmissionByPlayerDistance-Curve</param>
    private void MaterialShininess(float dist)
    {
        if(isActive)
        {
            emission = 10;
        }
        else
        {
            // Takes the y-axis value from the curve at the position of the given x-axis value
            emission = -dist * 0.5f; //lightEmissionByPlayerDistance.Evaluate(dist);
        }

        Debug.Log(emission);
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
        if(dist<=20)
        {
            // Calls the MaterialShininess funktion with the current distance
            MaterialShininess(dist);
        }
        // If the players is to far but the light stone is still glowing...
        else if(emission != -10)
      {
            // Calls the MaterialShininess Funktion with the y-axis value of the last Key in the curve
            MaterialShininess(20);// lightEmissionByPlayerDistance.keys[lightEmissionByPlayerDistance.keys.Length - 1].time);
      }
    }

    private void RaycastShot()
    {
        RaycastHit hit;
        //Debug.DrawLine(transform.position, transform.position + (transform.TransformDirection(Vector3.forward) * 2000), Color.red, float.PositiveInfinity);
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, int.MaxValue))
        {
            lRend.SetPosition(1, hit.point);

            if(hit.collider.gameObject.CompareTag("Prisma"))
            {
                if(currentTargetPrisma == null)
                {
                    currentTargetPrisma = hit.collider.gameObject;
                    currentTargetPrisma.GetComponent<Prisma>().PrismaLight(gradient, widthCurve, beamMaterial);
                }

            }
            else
            {
                if(currentTargetPrisma != null)
                {
                    currentTargetPrisma.GetComponent<Prisma>().PrismaLight(gradient, widthCurve, beamMaterial);
                    currentTargetPrisma = null;
                }
            }
        }
    }
}
