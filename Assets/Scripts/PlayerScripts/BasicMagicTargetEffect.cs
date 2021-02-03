using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMagicTargetEffect : MonoBehaviour
{
    [SerializeField]
    Transform player;

    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 TargetPos = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (rend.enabled)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("MagicInteractive"))
                {
                    transform.position = hit.point;
                }
            }
        }
    }
}
