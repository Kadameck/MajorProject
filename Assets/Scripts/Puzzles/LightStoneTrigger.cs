using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger that recognizes whether the player has thrown basic magic on the lightstone
/// </summary>
public class LightStoneTrigger : MonoBehaviour
{
    private ShamanControl player;
    private NewLightstone newLightstoneScript;

    /// <summary>
    /// gets the LightStone script
    /// </summary>
    private void Start()
    {
        newLightstoneScript = GetComponent<NewLightstone>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ShamanControl>();
    }

    /// <summary>
    /// Calls the Lighstone to turn active if the player use basic magic on the stone
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BasicMagic") && !player.getUseMagic())
        {
            //ls.ChangeLineActiveState();
            newLightstoneScript.ChangeActivState();
        }
    }
}
