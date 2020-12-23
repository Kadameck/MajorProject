using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbStageTransition : MonoBehaviour
{
    private OrbFollowPath oFP;

    private void Start()
    {
        oFP = GetComponent<OrbFollowPath>();
    }

    /// <summary>
    /// Starts the right transition
    /// </summary>
    /// <param name="stageIndex">1 = </param>
    public void StartTransition(int stageIndex)
    {
        if(stageIndex == 1)
        {
            LightstoneTransition();
        }
        else if (stageIndex == 2)
        {
            RiverTransition();
        }
        else if (stageIndex == 3)
        {
            GroveTRansition();
        }
        else
        {
            throw new System.Exception("Wrong stage transition Index (Orb)");
        }
    }

    /// <summary>
    /// The Orb is now at the serpentin Lightstone
    /// </summary>
    private void LightstoneTransition()
    {
        Debug.Log("Bin am Lichtstein");
    }

    /// <summary>
    /// The Orb is now at the river crossing spot
    /// </summary>
    private void RiverTransition()
    {
        Debug.Log("Bin am Fluss");
    }

    /// <summary>
    /// The last stage is comleted
    /// </summary>
    private void GroveTRansition()
    {
        Debug.Log("Bin Tot");
        Destroy(this.gameObject);
    }
}
