using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField]
    ShamanControl sc;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") && sc.GetIsGrounded() == false)
        {
            sc.SetGrounded(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") && sc.GetIsGrounded() == true)
        {
            sc.SetGrounded(false);
        }
    }
}
