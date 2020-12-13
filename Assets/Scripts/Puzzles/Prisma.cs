using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prisma : MonoBehaviour
{
    [SerializeField, Tooltip("Should the rotation of the prism be tracked")]
    bool trackRotation;
    [SerializeField, Tooltip("Optional\nIf no target is given, the prism directs the light in the direction which is forward with its current rotation.")]
    Transform target;

    private LineRenderer lRend;

    private void Start()
    {
        this.gameObject.AddComponent<LineRenderer>();
        lRend = GetComponent<LineRenderer>();
        lRend.positionCount = 2;
        lRend.SetPosition(0, transform.position);

        if(target != null)
        {
            transform.LookAt(target.position);
        }

        lRend.enabled = false;
    }

    private void Update()
    {
        if (lRend.enabled && trackRotation)
        {
            lRend.SetPosition(1, RaycastShot());
        }
    }

    public void PrismaLight(Gradient gradient, AnimationCurve widthCurve, Material beamMaterial)
    {
        if (!lRend.enabled)
        {
            lRend.widthCurve = widthCurve;
            lRend.colorGradient = gradient;
            lRend.material = beamMaterial;
            lRend.SetPosition(1, RaycastShot());
            lRend.enabled = true;
        }
        else
        {
            lRend.enabled = false;
        }
    }

    private Vector3 RaycastShot()
    {
        RaycastHit hit;
        //Debug.DrawLine(transform.position, transform.position + (transform.TransformDirection(Vector3.forward) * 2000), Color.red, float.PositiveInfinity);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, int.MaxValue))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
