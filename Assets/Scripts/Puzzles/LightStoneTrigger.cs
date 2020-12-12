using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStoneTrigger : MonoBehaviour
{
    private LightStone ls;

    private void Start()
    {
        ls = GetComponent<LightStone>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BasicMagic"))
        {
            ls.ChangeLineActiveState();
        }
    }
}
