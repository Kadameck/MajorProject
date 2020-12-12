using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStone : MonoBehaviour
{
    [SerializeField, Tooltip("Should the line update when the start or end point moves?")]
    bool trackMovement = false;   
    [SerializeField]
    Transform beamStart;
    [SerializeField]
    Transform beamTarget;
    [SerializeField]
    Material beamMaterial;
    [SerializeField]
    Color startColor;
    [SerializeField]
    Color endColor;
    [SerializeField, Range(0,1), Tooltip("Alpha Valuefrom 0 is invisible")]
    float alpha = 1.0f;

    private LineRenderer lRend;

    // Start is called before the first frame update
    void Start()
    {
        lRend = GetComponent<LineRenderer>();

        InitializeLine();
        UpgradeStartAndTarget();
    }

    private void Update()
    {
        if(trackMovement)
        {
            UpgradeStartAndTarget();
        }
        else
        {
            Destroy(this.GetComponent<LightStone>());
        }
    }

    private void InitializeLine()
    {
        lRend.positionCount = 2;
        lRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lRend.material = new Material(beamMaterial);

        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
                         new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) });

        lRend.colorGradient = gradient;

    }
    private void UpgradeStartAndTarget()
    {
        if(lRend.GetPosition(0) != beamStart.position || lRend.GetPosition(1) != beamTarget.position)
        {
            lRend.positionCount = 2;
            lRend.SetPosition(0, beamStart.position);
            lRend.SetPosition(1, beamTarget.position);
        }
    }
}
