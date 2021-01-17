using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbStageTransition : MonoBehaviour
{
    [SerializeField]
    NewLightstone serpentinLightstone;
    [SerializeField]
    GameObject orbitCenter;
    [SerializeField]
    Transform stageTwoContainerOneWP;
    [SerializeField]
    ShamanControl player;
    [SerializeField]
    Transform mainCamera;

    private OrbFollowPath oFP;
    private bool stopOrbit = false;

    private void Start()
    {
        oFP = GetComponent<OrbFollowPath>();
    }

    /// <summary>
    /// Starts the right transition
    /// </summary>
    /// <param name="stageIndex"></param>
    public void StartTransition(int stageIndex)
    {
        if(stageIndex == 0)
        {
            BeginnigTransition();
        }
        else if(stageIndex == 1)
        {
            LightstoneTransition();
        }
        else if (stageIndex == 2)
        {
            RiverTransition();
        }
        else if (stageIndex == 3)
        {
            MissingPrismTransition();
        }
        else if (stageIndex == 4)
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

    private void BeginnigTransition()
    {
        player.MakeControlable();
        oFP.StartNextStage();
    }

    /// <summary>
    /// The Orb is now at the serpentin Lightstone
    /// </summary>
    private void LightstoneTransition()
    {
        StartCoroutine(MoveAroundLightstone());
        //oFP.StartNextStage();
    }

    // eine eigene Update funktion die erst den orb neben dem center positioniert und anschliesend das center umrunden lässt
    IEnumerator MoveAroundLightstone()
    {
        // der orb soll sich jeden frame dem center weiter nähern solange er nicht einen abstand von 2 oder weniger hat
        while(Vector3.Distance(orbitCenter.transform.position, transform.position) >= 4.0f)
        {
            transform.LookAt(orbitCenter.transform.position);
            transform.Translate(Vector3.forward * Time.deltaTime * 4);
            yield return new WaitForEndOfFrame();
        }

        // Wird solange ein mal pro frame ausgeführt bis der lichtstein aktiviert wurde
        while(!stopOrbit)
        {
            // Rotiert den orb um einen gegebenen punkt
            // der abstand ist dabei der radius. also wenn der orb 2 unity vom oribt enter entfernt ist, ist der radius des kreises der bei der rotation entshet 2
            // Hier sthet also: rotiere um den orbitCenter, rotiere um die y achse, rotiere 180grad pro sekunde (also ein halber kreis pro sekunde)
            this.transform.RotateAround(orbitCenter.transform.position, new Vector3(0.0f, 1.0f, 0.0f), 180.0f * Time.deltaTime);
            stopOrbit = serpentinLightstone.GetIsActive();
            
            yield return new WaitForEndOfFrame();
        }


        //moveToStage2 = true;

        // Bewegt sich auf die position des ersten waypoints des ersten containers in stage 2 zu
        while(Vector3.Distance(stageTwoContainerOneWP.position, transform.position) >= 1.0f)
        {
            transform.LookAt(stageTwoContainerOneWP.position);
            transform.Translate(Vector3.forward * Time.deltaTime * 8);
            yield return new WaitForEndOfFrame();
        }

        // beginnt die verfolgung der nächsten stage
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
