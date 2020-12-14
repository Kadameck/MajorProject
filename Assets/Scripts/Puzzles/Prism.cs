using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A prism that, when activated by a light beam, can redirect this light beam in any other direction.
/// </summary>
public class Prism : MonoBehaviour
{
    [SerializeField, Tooltip("Target of the light beam\nLeaving this field empty Causes the light beam to go straight on based on the current orientation of the prism.\nAttention: This function overrides the rotation of the prism so that it is always aligned with the specified target. However, it can still be postponed.")]
    Transform target;
    [SerializeField, Tooltip("Should the light beam adjust as the prism moves or rotates?\nAttention: Rotating the prism does not work if a specific target has been specified. The Rotation of the prism will always orient itself towards this target.")]
    bool dynamicLightBeam;

    // LineRenderer of this gameobject
    private LineRenderer lRend;
    // The other Prism that is aktive just because of this prism
    private GameObject currentTargetPrism;
    
    /// <summary>
    /// Creates a LineRenderer and make it disabled
    /// Gives this Object also the Prisma tag
    /// </summary>
    private void Awake()
    {
        this.gameObject.tag = "Prisma";

        lRend = this.gameObject.AddComponent<LineRenderer>();
        lRend.enabled = false;

        // Aligns the prism so that it points to its target from the start
        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    /// <summary>
    /// Updates the lightbeam every frame if dynamicLightBeam is true and the lightbeam is activ
    /// </summary>
    private void Update()
    {
        if (dynamicLightBeam && lRend.enabled)
        {
            SetStartAndTarget();
        }
    }

    /// <summary>
    /// Starts the lightbeam
    /// </summary>
    /// <param name="lRenderer">The LineRenderer Component of the lightStone or other prism that activates this prism</param>
    public void SetPrismActive(LineRenderer lRenderer)
    {
        // Sets all light beam properties to match those of the object that activates this prism
        lRend.colorGradient = lRenderer.colorGradient;
        lRend.widthCurve = lRenderer.widthCurve;
        lRend.material = lRenderer.material;
        // Gives the lightbeam two slots for points (start and end)
        lRend.positionCount = 2;

        // Calls the SetStartAndTarget Function
        SetStartAndTarget();
        // Make the LineRenderer of this prism visible
        lRend.enabled = true;
    }

    /// <summary>
    /// Deactivates the LineRenderer of this prism
    /// </summary>
    public void SetPrismDeactive()
    {
        // Make the LinRenderer of this prism invisible
        lRend.enabled = false;

        // If this Prism has a target the target will also be deactivated 
        if(currentTargetPrism != null)
        {
            currentTargetPrism.GetComponent<Prism>().SetPrismDeactive();
            currentTargetPrism = null;
        }
    }

    /// <summary>
    /// Defines the start and target points of the lightbeam
    /// </summary>
    private void SetStartAndTarget()
    {
        // Checks if the start position has changed
        if (transform.position != lRend.GetPosition(0))
        {
            // Sets the new start position
            lRend.SetPosition(0, transform.position);
        }

        // Checks if there is a defined target
        if(target == null)
        {
            // Gets the endpoint of the light beam
            Vector3 targetPos = FindTargetPos();
            // Sets the endpoint of the light beam
            lRend.SetPosition(1, targetPos);
        }
        else
        {
            // Rotates to face the target
            transform.LookAt(target.position);
            // Gets the endpoint of the light beam
            Vector3 targetPos = FindTargetPos();
            // Sets the endpoint of the light beam
            lRend.SetPosition(1, targetPos);
        }
    }

    /// <summary>
    /// Defines the endpoint of the lightbeam
    /// </summary>
    /// <returns>The Coordinates of the endpoint of the light beam</returns>
    private Vector3 FindTargetPos()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5000))
        {
            // Hitted object is another prism
            if (hit.collider.gameObject.CompareTag("Prisma"))
            {
                // Checks if this prism currently has no other prism activated 
                if(currentTargetPrism == null)
                {
                    // Checks if the other prism has no other prisms itself
                    if (hit.collider.gameObject.GetComponent<Prism>().GetCurrentTargetPrism() == null)
                    {
                        // The hitten prism will be set as currently activated prism
                        currentTargetPrism = hit.collider.gameObject;
                        // The hitte
                        currentTargetPrism.GetComponent<Prism>().SetPrismActive(lRend);

                    }
                }
                // Checks if the hitten prism is a new one
                else if(currentTargetPrism != hit.collider.gameObject)
                {
                    // Checks whether the prism hit is different from the one that activated this prism (prevents two prisms from (de-) activating each other
                    if (hit.collider.gameObject.GetComponent<Prism>().GetCurrentTargetPrism() == null)
                    {
                        // Deactivates the current activ prism
                        currentTargetPrism.GetComponent<Prism>().SetPrismDeactive();
                        // Sets the hitten prism as new activ prism
                        currentTargetPrism = hit.collider.gameObject;
                        // Activates the new hitten prism
                        currentTargetPrism.GetComponent<Prism>().SetPrismActive(lRend);
                    }
                }
            }
            // No prism hitten
            else
            {
                // Checks whether there is currently a prism which has been activated by this prism
                if (currentTargetPrism != null)
                {
                    // Deactivates the activ prism
                    currentTargetPrism.GetComponent<Prism>().SetPrismDeactive();
                    // Resets the currentlytargetetPrism to null
                    currentTargetPrism = null;
                }
            }
        }

        // Returns the hitpoint of the raycast
        return hit.point;
    }

    /// <summary>
    /// Gives the currenttargetPrism
    /// </summary>
    /// <returns>currenttargetPrism</returns>
    public GameObject GetCurrentTargetPrism()
    {
        return currentTargetPrism;
    }
}
