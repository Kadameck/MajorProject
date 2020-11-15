using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform target;

    [SerializeField]
    Vector3 cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        cameraPosition = new Vector3(0.0f, 27.0f, 13.0f);
    }


    void FixedUpdate()
    {
        // Let the camera follow the palyer movement in a certain zoom distance
        transform.position = new Vector3(cameraPosition.x + target.position.x, cameraPosition.y + target.position.y, -cameraPosition.z + target.position.z);
        transform.LookAt(target.position);
    }
}
