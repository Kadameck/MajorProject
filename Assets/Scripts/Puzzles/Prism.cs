using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    private LineRenderer lineRend;
    private Material mat;

    // Für den fall das der Lichtstein der diesem prisma hier als quelle dient bereits von anfang an aktiv ist
    // muss dashier Awake sein. denn der lichtstein wird in der start initialisiert, sprich dieses prisma hier muss noch davor bereit sein
    void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 2;
        lineRend.SetPosition(0, this.transform.position);
        lineRend.SetPosition(1, target.transform.position);
        mat = GetComponent<Renderer>().material;
        mat.DisableKeyword("_EMISSION");
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
        if (target.GetComponent<Prism>() != null)
        {
            target.GetComponent<Prism>().SetPrismActiveOrDeactive(lineRend, lineRend.enabled);
        }
    }
}
