/****************************************************************************************************************************
 *                                                                                                                          *
 * Some parts of this script are from Sebastian Lague and were taken from the following tutorial playlist:                  *
 * https://www.youtube.com/watch?v=rQG9aUWarwE&list=PLFt_AvWsXl0dohbtVgHDNmgZV_UY7xZv7                                      *
 * Last accessed: 28.11.2020                                                                                                *
 *                                                                                                                          *
 * **************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanFildOfView : MonoBehaviour
{
    [Tooltip("The radius of the field of view")]
    public float viewRadius;
    [Tooltip("The angle of the fild of view")]
    [Range(0.0f, 360.0f)]
    public float viewAngle;
    [Tooltip("The PointLight that visualize the players field of view")]
    public Light pointLight;
    
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    [Space(10)]

    [Tooltip("A higher value will result in a more precise scan of the surroundings. But causes more Raycasts")]
    public float meshResolution;
    [Tooltip("A higher value will result in a more precise track of the edges of the obstacles. But causes more Raycasts")]
    public float edgeDetectionAccuracy;
    public float edgeDistanceThreshold;
    [Space(10)]

    public MeshFilter viewAreaMeshFilter;

    private List<Transform> visibleTargetsList = new List<Transform>();
    private Mesh viewAreaMesh;

    private ShamanControl sControl;
    private float normalViewRadius;
    private float normalPointLightRange;

     private void Start()
     {
        // Creates a new mesh
        viewAreaMesh = new Mesh();
        // Names the new mesh in the hierachy
        viewAreaMesh.name = "FieldOfViewMesh";
        // Sets the new mesh as the mesh of the mesh filter
        viewAreaMeshFilter.mesh = viewAreaMesh;

        sControl = GetComponent<ShamanControl>();

        normalViewRadius = viewRadius;
        normalPointLightRange = pointLight.range;

         //StartCoroutine("FindTargetsWithDelay", 0.2f);
    }
    //
    // IEnumerator FindTargetsWithDelay(float delay)
    // {
    //     while(true)
    //     {
    //         yield return new WaitForSeconds(delay);
    //         ResetVisibleTargets();
    //         FindVisibleTargets();
    //         ShowVisibleTargets();
    //     }
    // }
    private void FixedUpdate()
    {
        ResetVisibleTargets();
        FindVisibleTargets();
    }

    /// <summary>
    /// Creates the viewAreaMesh after the movement calculation in each frame
    /// </summary>
    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    public Vector3 CalculateDirFromAngle(float angleInDegree, bool angleIsGlobal)
    {
        // Checks if the angle should be in reference to the global coordinate system ore the local of the player
        if (!angleIsGlobal)
        {
            angleInDegree += transform.eulerAngles.y;
        }

        // The direction of the raycasts. The y-Value times 2 because the actual mesh will be set above the drawn fov in the editor
        return new Vector3(Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegree * Mathf.Deg2Rad));
    }

    void FindVisibleTargets()
    {
        ResetVisibleTargets();

        // Finds all Objects with the targetLayer in a circle around the player, with a radius of the viewRadius
        Collider[] targetsInViewradius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewradius.Length; i++)
        {
            // Takes the transform of the targets in the array
            Transform target = targetsInViewradius[i].transform;

            // Calculates the angle between the looking direction of the player and the direction of the target from the players position
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Checks if the target is outside the viewAngle
            if (Vector3.Angle(dirToTarget, transform.forward) < viewAngle * 0.5f)
            {
                // calculates the distance between the player and the target
                float distToTarget = Vector3.Distance(transform.position, target.position);

                // Shoots a raycast from the player to the target an checks if there is no object with the obstacle layer is between them
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargetsList.Add(target);
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        // Calculates the number of rays that will be shoot in the field of view
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);

        // Calculates the angle between each ray to another in the fov
        float stepAngleValue = viewAngle / rayCount;
        // A list to store all hit points from  the raycasts in the fov
        List<Vector3> viewRayHitPoints = new List<Vector3>();

        // A Variable to store the information of the counter clockwise naber raycast of the current ray (in the following loop)
        ViewCastInfo previousRay = new ViewCastInfo();

        // Loops throug all raycasts in the fov
        for (int i = 0; i <= rayCount; i++)
        {
            // Calculates the angles of each ray itself in the fov
            float angle = transform.eulerAngles.y - viewAngle * 0.5f + stepAngleValue * i;

            // Creates a new ViewcastInfo object for the current Ray throug the ViewCast Methode
            ViewCastInfo newRayast = ViewCast(angle);

            if(i > 0)
            {
                bool edgeThresholdExceeded = Mathf.Abs(previousRay.rayLenght - newRayast.rayLenght) > edgeDistanceThreshold;

                if(previousRay.hitPoint != newRayast.hitPoint || (previousRay.hitSomething && newRayast.hitSomething && edgeThresholdExceeded))
                {
                    ObstacleEdgeInfo obstacleEdge= FindObstacleEdge(previousRay, newRayast);

                    // Adds the new RaycastHits to the list
                    if(obstacleEdge.closestPointWhoHit != Vector3.zero)
                    {
                        viewRayHitPoints.Add(obstacleEdge.closestPointWhoHit);
                    }

                    if (obstacleEdge.closestPointWhoMissed != Vector3.zero)
                    {
                        viewRayHitPoints.Add(obstacleEdge.closestPointWhoMissed);
                    }
                }
            }

            viewRayHitPoints.Add(newRayast.hitPoint);

            previousRay = newRayast;
        }

        // The mesh is made up of several triangles.
        // Each conection is as follows: "the position of the player" -> "hitpoint of a ray" -> "hitpoint of the ray clockwise next to it" -> "position of the player".
        // The number of vertex points is therefore the number of hit points + the player position
        int viewAreaMeshVertexCount = viewRayHitPoints.Count + 1;

        // Sets the Vertice and triangle information of the viewAreaMesh
        Vector3[] viewAreaMeshVertices = new Vector3[viewAreaMeshVertexCount];
        int[] viewAreaMeshTriangles = new int[(viewAreaMeshVertexCount - 2) * 3];

        // Setzs the first vertice of the view area mesh to the players position
        viewAreaMeshVertices[0] = Vector3.zero;

        // Loops throug each Vertex (the first one in already set. Thats why -1)
        for (int i = 0; i < viewAreaMeshVertexCount - 1; i++)
        {
            // Sets the next vertex position in the array (+1 because the index 0 is already set) to the i-th position in the viewRayHitPoints list
            // These i-th point will be calculated relative to the transform of the player
            viewAreaMeshVertices[i + 1] = transform.InverseTransformPoint(viewRayHitPoints[i]);

            //Prevents a out of bounds exeption error
            if (i < viewAreaMeshVertexCount - 2)
            {
                // Creates the triangles of the field of view mesh
                viewAreaMeshTriangles[i * 3] = 0;
                viewAreaMeshTriangles[i * 3 + 1] = i + 1;
                viewAreaMeshTriangles[i * 3 + 2] = i + 2;
            }
        }

        // Sends the mesh informations to the mesh and recalculates it
        viewAreaMesh.Clear();
        viewAreaMesh.vertices = viewAreaMeshVertices;
        viewAreaMesh.triangles = viewAreaMeshTriangles;
        viewAreaMesh.RecalculateNormals();
    }

    ObstacleEdgeInfo FindObstacleEdge(ViewCastInfo closestPointWhoHit, ViewCastInfo closestPointWhoMissed)
    {
        float minAngle = closestPointWhoHit.rayAngle;
        float maxAngle = closestPointWhoMissed.rayAngle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        // Every iteration there will be cast a new Ray in the middle of the last ray that hits an obstacle and the first ray that missed that obstacle
        // This way the location of the edge of the obstacle will be detected each iteration more precisely
        for (int i = 0; i < edgeDetectionAccuracy; i++)
        {
            // Calculates a new angle in the middle between the hiting and de missing rayAngle
            float newAngle = (minAngle + maxAngle) * 0.5f;
            // Cast a new Raycast at the calculatet angle
            ViewCastInfo newViewCast = ViewCast(newAngle);

            bool edgeThresholdExceeded = Mathf.Abs(closestPointWhoHit.rayLenght - closestPointWhoMissed.rayLenght) > edgeDistanceThreshold;

            // Checks if the new raycast hits the obstacle 
            if (newViewCast.hitSomething == closestPointWhoHit.hitSomething && !edgeThresholdExceeded)
            {
                // If the new Ray hits the obstacle these will be set as new closesdPointWhoHit
                minAngle = newAngle;
                minPoint = newViewCast.hitPoint;
            }
            else
            {
                // If the new Ray not hits the obstacle these will be set as new closesdPointWhoMissed
                maxAngle = newAngle;
                maxPoint = newViewCast.hitPoint;
            }
        }

        // Returns the information of the closesdPointWhoHit and the closesdPointWhoMissed as a ObstacleEdgeInfo struct
        return new ObstacleEdgeInfo(minPoint, maxPoint);
    }

    /// <summary>
    /// Handles the raycats in the fild of view
    /// </summary>
    /// <param name="globalAngle">the current raycasts angle in the fov</param>
    /// <returns></returns>
    ViewCastInfo ViewCast(float globalAngle)
    {
        // Gets the direction of the ray
        Vector3 dir = CalculateDirFromAngle(globalAngle, true);

        // Stores the shooting result of the ray
        RaycastHit rayHit;

        // Wenn schleichen und aktiver noch nicht der verringerte ist
        if(sControl.Sneak() && viewRadius != normalViewRadius * 0.5f)
        {
            // aktiver radius wird verringert
            viewRadius = normalViewRadius * 0.5f;
            pointLight.range = normalPointLightRange * 0.5f;
        }
        // wenn sneak false und der aktive radius verkleinert ist
        else if(!sControl.Sneak() && viewRadius != normalViewRadius)
        {
            // aktiver radius soll normalisirt werden
            viewRadius = normalViewRadius;
            pointLight.range = normalPointLightRange;
        }

        // Shoots the ray and checks if something is hiten
        if(Physics.Raycast(transform.position, dir, out rayHit, viewRadius, obstacleMask))
        {
            // Returns a new ViewCastInfo objekt with all the informations about the hit position
            return new ViewCastInfo(true, rayHit.point, rayHit.distance, globalAngle);
        }
        else
        {
            // Returns a new ViewCastInfo objekt with the information that nothing is hiten
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    private void ResetVisibleTargets()
    {
        // Clears the visibletargetsList to prevent duplicates
        visibleTargetsList.Clear();
    }

    /// <summary>
    /// Stores the raycasts Informations
    /// </summary>
    public struct ViewCastInfo
    {
        public bool hitSomething;
        public Vector3 hitPoint;
        public float rayLenght;
        public float rayAngle;

        // Constructor to set the variables
        public ViewCastInfo(bool _hitSomething, Vector3 _hitPoint, float _rayLenght, float _rayAngle)
        {
            hitSomething = _hitSomething;
            hitPoint = _hitPoint;
            rayLenght = _rayLenght;
            rayAngle = _rayAngle;
        }
    }

    /// <summary>
    /// Stores the raycast informations from the rays with the edge of an obstacle beween them
    /// </summary>
    public struct ObstacleEdgeInfo
    {
        public Vector3 closestPointWhoHit;
        public Vector3 closestPointWhoMissed;

        // Constructor to set the variables
        public ObstacleEdgeInfo(Vector3 _closestPointWhoHit, Vector3 _closestPointWhoMissed)
        {
            closestPointWhoHit = _closestPointWhoHit;
            closestPointWhoMissed = _closestPointWhoMissed;
        }
    }

    public List<Transform> GetVisibleTargetsList()
    {
        return visibleTargetsList;
    }
}
