using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    [SerializeField, Tooltip("Muss nur gegeben sein wenn das eigentliche target bewegtlich ist.")]
    GameObject alternativTarget;

    private bool isPlaced = false;
    private bool targetIsMovable = false;
    private bool waitForPlaceTarget = false;

    private LineRenderer lineRend;
    private Material mat;

    // Für den fall das der Lichtstein der diesem prisma hier als quelle dient bereits von anfang an aktiv ist
    // muss dashier Awake sein. denn der lichtstein wird in der start initialisiert, sprich dieses prisma hier muss noch davor bereit sein
    void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 2;

        // Prüft ob das ziel beweglich ist
        if (target.GetComponent<CarryObject>() != null)
        {
            targetIsMovable = true;
        }

        // setzt start und endpunkt des lichtstrahl auf die gleiche position (als platzhalter)
        lineRend.SetPosition(0, this.transform.position);
        lineRend.SetPosition(1, this.transform.position);

        mat = GetComponent<Renderer>().material;
        mat.DisableKeyword("_EMISSION");
    }

    private void Update()
    {
        // dieses prisma ist aktiv aber das target prisma ist noch nicht an seinem platz
        if(waitForPlaceTarget)
        {
            // ist das target prisma mittlerweile plaziert und dieses prisma hier immernoch aktiv?
            if(target.GetComponent<Prism>().GetIsPlaced() && lineRend.enabled)
            {
                // es wird nicht mehr auf die platzierung des target prismas gewartet und das target prisma kann aktiviert werden
                waitForPlaceTarget = false;
                ActivateOrDeactivateTarget();
            }
            // ist das targetprisma noch nicht plaziert und noch kein  ziel als endpunkt für den strahl gesetzt
            else if(lineRend.GetPosition(1) == this.transform.position)
            {
                // setzte das alternative ziel, als ziel
                lineRend.SetPosition(1, alternativTarget.transform.position);
            }
        }
    }

    // setzt den lichtstrahl des prismas auf den selben sichtbar/unsichtbar wert den der lichtstein bzw das andere prisma hat, von dem aus diese
    // funktion aufgerufen wurde
    // außerdem  erden die linerenderer einstellungen des lichtsteins/anderen prismas übernommen
    public void SetPrismActiveOrDeactive(LineRenderer lineRenderer, bool activeState)
    {

        lineRend.enabled = activeState;

        if(activeState)
        {
            lineRend.material = lineRenderer.material;
            lineRend.colorGradient = lineRenderer.colorGradient;
            lineRend.widthCurve = lineRenderer.widthCurve;
            mat.EnableKeyword("_EMISSION");
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
        }

        ActivateOrDeactivateTarget();
    }

    // wenn das target ein prisma ist, wird seine sichtbarkeit/unsichtbarkeit diesem priesma hier angepasst
    private void ActivateOrDeactivateTarget()
    {
        // ist das target ein prisma
        if (target.GetComponent<Prism>() != null)
        {
            // ist das target prisma tragbar?
            if(target.GetComponent<CarryObject>() != null)
            {
                // ist das target prisma plaziert?
                if (target.GetComponent<Prism>().GetIsPlaced())
                {
                    // setzt die position dieses prismas als startpunkt des lichtstrahl und die position des targets als endpunkt des lichtstrahls
                    lineRend.SetPosition(0, this.transform.position);
                    lineRend.SetPosition(1, target.transform.position);

                    // das target prisma kann aktiviert werden
                    target.GetComponent<Prism>().SetPrismActiveOrDeactive(lineRend, lineRend.enabled);
                }
                // das targetprisma ist noch nicht plaziert
                else
                {
                    waitForPlaceTarget = true;
                }
            }
            // das prisma ist nicht tragbar
            else
            {
                // setzt die position dieses prismas als startpunkt des lichtstrahl und die position des targets als endpunkt des lichtstrahls
                lineRend.SetPosition(0, this.transform.position);
                lineRend.SetPosition(1, target.transform.position);

                // das target prisma kann aktiviert werden
                target.GetComponent<Prism>().SetPrismActiveOrDeactive(lineRend, lineRend.enabled);
            }
        }
        // Das target ist kein prisma
        else
        {
            // setzt die position dieses prismas als startpunkt des lichtstrahl und die position des targets als endpunkt des lichtstrahls
            lineRend.SetPosition(0, this.transform.position);
            lineRend.SetPosition(1, target.transform.position);
        }
    }

    /// <summary>
    /// Da die kugel beim richtigen plazieren nicht wider nehmenbar wird, 
    /// ist es nicht nötig eine mechanik einzubeiden die is Placed false werden lässt
    /// </summary>
    public void SetIsPlaced()
    {
        isPlaced = true;
    }

    public bool GetIsPlaced()
    {
        return isPlaced;
    }
}
