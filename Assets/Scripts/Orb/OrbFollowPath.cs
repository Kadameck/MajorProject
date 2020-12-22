using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbFollowPath : MonoBehaviour
{
    // Die Objekte die die Waypoints enthalten
    [SerializeField]
    GameObject[] pathContainer; // routes

    int iterator = 0;

    private List<Vector3> positions = new List<Vector3>();
    private List<Vector3> gizmoPositions = new List<Vector3>();
    private Vector3 gizmoPos;
    private float t;
    private Vector3 orbPos;
    private float speed;
    private bool canStartNewCoroutine;
    private int c;
    
    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        speed = 0.5f;
        canStartNewCoroutine = true;

        // Loopt durch die container
        for (int container = 0; container < pathContainer.Length; container++)
        {
            // Loopt durch die Childobjekte des pathcontainers
            for (int waypoint= 0; waypoint < pathContainer[container].transform.childCount; waypoint++)
            {
                positions.Add(pathContainer[container].transform.GetChild(waypoint).position);
            }
        }
    }

    private void Update()
    {
        if (canStartNewCoroutine) { StartCoroutine(Follow()); }
    }

    IEnumerator Follow()
    {
        canStartNewCoroutine = !canStartNewCoroutine;

        Vector3 p0 = positions[iterator + 0];
        Vector3 p1 = positions[iterator + 1];
        Vector3 p2 = positions[iterator + 2];
        Vector3 p3 = positions[iterator + 3];
        
        iterator += 4;

        while(t < 1)
        {
            t += Time.deltaTime * speed;

            orbPos = Mathf.Pow(1 - t, 3) * p0 +
                       3 * Mathf.Pow(1 - t, 2) * t * p1 +
                       3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                       Mathf.Pow(t, 3) * p3;
            
            transform.position = orbPos;
            yield return new WaitForEndOfFrame();
        }

        t = 0f;

        if(iterator + 3 > positions.Count - 1)
        {
            iterator = 0;
        }

        canStartNewCoroutine = !canStartNewCoroutine;
    }

    private void OnDrawGizmos()
    {
        gizmoPositions.Clear();
        c = 0;

        for (int container = 0; container < pathContainer.Length; container++)
        {
            // Loopt durch die Childobjekte des pathcontainers
            for (int waypoint = 0; waypoint < pathContainer[container].transform.childCount; waypoint++)
            {
                gizmoPositions.Add(pathContainer[container].transform.GetChild(waypoint).position);
            }

            for (float i = 0; i <= 1; i += 0.05f)
            {
                gizmoPos = Mathf.Pow(1 - i, 3) * gizmoPositions[c] +
                           3 * Mathf.Pow(1 - i, 2) * i * gizmoPositions[c + 1] +
                           3 * (1 - i) * Mathf.Pow(i, 2) * gizmoPositions[c + 2] +
                           Mathf.Pow(i, 3) * gizmoPositions[c + 3];

                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(gizmoPos, 0.25f);

            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gizmoPositions[c], gizmoPositions[c + 1]);
            Gizmos.DrawLine(gizmoPositions[c + 2], gizmoPositions[c+ 3]);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(gizmoPositions[c + 1], 0.75f);
            Gizmos.DrawSphere(gizmoPositions[c + 2], 0.75f);
            
            c += 4;
        }
    }
}