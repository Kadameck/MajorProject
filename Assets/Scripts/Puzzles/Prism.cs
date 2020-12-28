using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    private LineRenderer lineRend;

    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.SetPosition(0, this.transform.position);
        lineRend.SetPosition(1, target.transform.position);
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
