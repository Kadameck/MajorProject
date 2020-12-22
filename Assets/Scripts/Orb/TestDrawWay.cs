using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawWay : MonoBehaviour
{
    public GameObject wpHolder;


   // private GameObject[] waypoints;

    int points = 50;
    int i = 0;

    Vector3 gizmoPositions;

    Vector3 p0;
    Vector3 p1;
    Vector3 p2;

    private void OnDrawGizmos()
    {
        for (int x = 0; x < wpHolder.transform.childCount; x += 2)
        {
            p0 = wpHolder.transform.GetChild(x).transform.position;
            p1 = wpHolder.transform.GetChild(x+1).transform.position;
            p2 = wpHolder.transform.GetChild(x+2).transform.position;

            for (float i = 0; i < points + 1; i++)
            {
                float t = i / (float)points;
                float u = 1 - t;
                float uSquare = u * u;
                float tSquare = t * t;

                Vector3 currentPosition = uSquare * p0;
                currentPosition += 2 * u * t * p1;
                currentPosition += tSquare * p2;

                gizmoPositions = currentPosition;
                Gizmos.color = Color.red;

                Gizmos.DrawSphere(gizmoPositions, 0.25f);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
        }
    }
}
