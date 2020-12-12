using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The light stones the player can activate through basic magic
/// </summary>
public class LightStone : MonoBehaviour
{
    [SerializeField, Tooltip("Waht should be the start state of this light stone?")]
    bool isActive = false;
    [SerializeField, Tooltip("Should the line update when the start or end point moves?")]
    bool trackMovement = false;   
    [SerializeField, Tooltip("Set here the start transfom of the lightbeam \n If you leave this field empty, the position of the object that this script attached will be used")]
    Transform beamStart;
    [SerializeField, Tooltip("Set here the targets transform of the lightbeam")]
    Transform beamTarget;
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
    // Default initialized stat
    private bool isInitialized = false;
    // Material used by this Object
    private Material mat;
    // default emission value
    private float emission = -10;
    // Player Object
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
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
            UpgradeStartAndTarget();
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
            UpgradeStartAndTarget();
        }

        // Upgrades the start and/or target position of the line if it is nessesery
        if(trackMovement && lRend.enabled)
        {
            UpgradeStartAndTarget();
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

        // Sets the color of the line (start and target point)
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
                         new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) });
        lRend.colorGradient = gradient;

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
    /// Sets the start and target point of the LineRenderer
    /// </summary>
    private void UpgradeStartAndTarget()
    {
        // Checks if the start and target positions has changed 
        if(lRend.GetPosition(0) != beamStart.position || lRend.GetPosition(1) != beamTarget.position)
        {
            // Sets the number of points the lineRenderer should conect to two
            lRend.positionCount = 2;

            // Sets the start and target positions for the 2 given points that the lineRenderer should conect
            lRend.SetPosition(0, beamStart.position);
            lRend.SetPosition(1, beamTarget.position);
        }
    }

    /// <summary>
    /// Controlls the emission value of the material
    /// </summary>
    /// <param name="dist">x-axis value of the lightEmissionByPlayerDistance-Curve</param>
    private void MaterialShininess(float dist)
    {
        // Takes the y-axis value from the curve at the position of the given x-axis value
        emission = lightEmissionByPlayerDistance.Evaluate(dist);
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
        if (dist <= lightEmissionByPlayerDistance.keys[lightEmissionByPlayerDistance.keys.Length-1].time)
        {
            // Calls the MaterialShininess funktion with the current distance
            MaterialShininess(dist);
        }
        // If the players is to far but the light stone is still glowing...
        else if(emission != -10)
        {
            // Calls the MaterialShininess Funktion with the y-axis value of the last Key in the curve
            MaterialShininess(lightEmissionByPlayerDistance.keys[lightEmissionByPlayerDistance.keys.Length - 1].time);
        }
    }
}
