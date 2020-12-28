using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLightstone : MonoBehaviour
{
    [SerializeField]
    bool isActiv = false;
    [SerializeField]
    GameObject laserBeamTarget;

    private GroveDoorMechanism doorMech;
    private LineRenderer lineRenderer;


    private void Start()
    {
        doorMech = FindObjectOfType<GroveDoorMechanism>();
        lineRenderer = GetComponent<LineRenderer>();
        SetLaserBeamStartAndEnd();

        // Wenn der stein bereits von beginn an aktiv sein soll
        if (isActiv)
        {
            // im GroveDoorMechanism.cs wird der counter für active steine um eins erhöht
            doorMech.IncreaseActiveLightstoneCounter();
            ChangeLaserBeamVisibility();
        }
    }

    // Sets the laser beam start and end point
    private void SetLaserBeamStartAndEnd()
    {
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, laserBeamTarget.transform.position);

    }

    // macht den laser sichtbar oder unsichtbar, je nachdem ob der lichtstein aktiv ist oder nicht
    private void ChangeLaserBeamVisibility()
    {
        lineRenderer.enabled = isActiv;
        ActivateOrDeactivateTarget();
    }

    // setzt den laserstrahl des prismas (solte eines als target benutzt werden) auf den welben sichtbar/unsichtbar wert wie der lichtstein seinen hat
    private void ActivateOrDeactivateTarget()
    {
        // Prüft ob das target eine prisma script componente hat
        if (laserBeamTarget.GetComponent<Prism>() != null)
        {
            // macht den lichtstrahl des prismas sichtbar/unsichtbar, je nachdem ob der lichtstein aktiv ist oder nicht
            laserBeamTarget.GetComponent<Prism>().SetPrismActiveOrDeactive(lineRenderer, isActiv);
        }
    }

    /// <summary>
    /// Ändert den zustand des lichtsteins
    /// Wird vom LightstoneTrigger.cs aus aufgerufen
    /// </summary>
    public void ChangeActivState()
    {
        isActiv = !isActiv;
        ChangeLaserBeamVisibility();

        // je nachdem ob der lichtstein gerade aktiviert oder deaktiviert wurde, wird im GroveDoorMechanism der counter der aktiven lichtsteine um eins erhöht oder verringert
        if(isActiv)
        {
            doorMech.IncreaseActiveLightstoneCounter();
        }
        else
        {
            doorMech.DegreaseActiveLightstoneCounter();
        }
    }

    public bool GetIsActive()
    {
        return isActiv;
    }
}
