using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPathVisualization : MonoBehaviour
{
    public GameObject[] wp;
    private List<Vector3> gizmoPositions = new List<Vector3>();
    private Vector3 gizmoPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        gizmoPositions.Clear();
        gizmoPositions.Add(wp[0].transform.position);
        gizmoPositions.Add(wp[1].transform.position);
        gizmoPositions.Add(wp[2].transform.position);
        gizmoPositions.Add(wp[3].transform.position);

        for (float i = 0; i <= 1; i += 0.05f)
        {
            gizmoPos = Mathf.Pow(1 - i, 3) * gizmoPositions[0] +
                       3 * Mathf.Pow(1 - i, 2) * i * gizmoPositions[1] +
                       3 * (1 - i) * Mathf.Pow(i, 2) * gizmoPositions[2] +
                       Mathf.Pow(i, 3) * gizmoPositions[3];

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(gizmoPos, 0.25f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(gizmoPositions[0], gizmoPositions[1]);
        Gizmos.DrawLine(gizmoPositions[2], gizmoPositions[3]);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(gizmoPositions[1], 0.75f);
        Gizmos.DrawSphere(gizmoPositions[2], 0.75f);
    }
}
