using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbFollowPath : MonoBehaviour
{
    // Die Objekte die die Waypoints enthalten
    [SerializeField]
    GameObject[] pathContainer;

    private List<Vector3> positions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        // Loopt durch die Childobjekte des ersten pathcontainers
        for (int i = 1; i < pathContainer[0].transform.childCount; i++)
        {
            // fügt die childobjekte der liste zu damit man einen start zum berechnen hat
            // Wird dann später das letzte child ereicht muss diese liste gecleard und mit den positionen der childern des nächsten pathContainer befüllt werden
            positions.Add(pathContainer[0].transform.GetChild(i).position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
