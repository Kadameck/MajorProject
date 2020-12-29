using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePrismDestinationSpot : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Prism>() != null)
        {
            // setzt das prisma an die stelle des spots und um 1 hoch weil der pivot nicht ganz passt
            other.gameObject.transform.position = transform.position + Vector3.up;
            Destroy(other.gameObject.GetComponent<CarryObject>());
            Destroy(other.gameObject.GetComponent<Rigidbody>());
            // Setzt die is Placed variable des prismas auf true
            other.gameObject.GetComponent<Prism>().SetIsPlaced();
        }
    }
}
