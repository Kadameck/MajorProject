using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrySphere : MonoBehaviour
{
    private bool targedReached = false;
    private Transform player;
    private Vector3 pDP;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(pDP != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, pDP, Time.deltaTime * 3);
            if(Vector3.Distance(transform.position, pDP) < 0.1f)
            {
                transform.position = pDP;
                SetTargetReached();
            }
        }
        else
        {
            Vector3 targetPoint = (((transform.position - player.position).normalized) + Vector3.up) + player.position;
            // Moves the sphere normal speed
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * 3);
        }
    }

    public void SetPutDownPlace(Vector3 putDownPlace)
    {
        pDP = putDownPlace;
    }

    private void SetTargetReached()
    {
        targedReached = true;
    }

    public bool GetTargetReached()
    {
        return targedReached;
    }
}
