using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    [SerializeField]
    float orbSpeed = 0.1f;

    [SerializeField]
    List<Transform> wayPoints;

    private Transform orbTrans;
    private Transform playerTrans;
    private Rigidbody rb;
    private int iterator = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        orbTrans = GameObject.FindGameObjectWithTag("Orb").transform;
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        rb = orbTrans.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newDirection = Vector3.zero;

        if (Vector3.Distance(orbTrans.position, playerTrans.position) < 20)
        {
            orbTrans.LookAt(wayPoints[iterator].position);
            orbTrans.Translate(new Vector3(0.0f, 0.0f, Time.deltaTime * orbSpeed));

            if (Vector3.Distance(orbTrans.position, wayPoints[iterator].position) < 2)
            {
                if(iterator == wayPoints.Count-1)
                {
                    Destroy(orbTrans.gameObject);
                    Destroy(this);
                }
                else
                {
                    iterator++;
                }
            }
        }
    }
}
