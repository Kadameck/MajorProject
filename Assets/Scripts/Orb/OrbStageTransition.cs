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
        if(stageIndex == 0)
        {
            LightstoneTransition();
        }
        else if (stageIndex == 1)
        {
            RiverTransition();
        }
        else if (stageIndex == 2)
        {
            MissingPrismTransition();
        }
        else if (stageIndex == 3)
        {
            GroveTransition();
        }
        else
        {
            throw new System.Exception("Wrong stage transition Index (Orb)");
        }
    }

    /****************************************************************************************************************************************************************************
     *                                                                                                                                                                          *
     *             In den XYZTransition Funktionen können jetzt die verhalten programmiert werden die der orb haben soll ehe er in die nächste stage welchselt                  *
     *                                                                                                                                                                          *
     ***************************************************************************************************************************************************************************/ 

    /// <summary>
    /// The Orb is now at the serpentin Lightstone
    /// </summary>
    private void LightstoneTransition()
    {
        Debug.Log("Bin am Lichtstein");
        oFP.StartNextStage();
    }

    /// <summary>
    /// The Orb is now at the river crossing spot
    /// </summary>
    private void RiverTransition()
    {
        Debug.Log("Bin am Fluss");
        oFP.StartNextStage();
    }

    private void MissingPrismTransition()
    {
        Debug.Log("Bin am fehlenden Prisma");
        oFP.StartNextStage();
    }

    /// <summary>
    /// The last stage is comleted
    /// </summary>
    private void GroveTransition()
    {
        Debug.Log("Bin Tot");
        Destroy(this.gameObject);
    }
}
