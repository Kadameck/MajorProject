using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMagicTargetEffect : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [HideInInspector]
    public bool targetIsNotGroundOrRightHight = false;

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

        if (rend.enabled || targetIsNotGroundOrRightHight)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("MagicInteractive")) &&
                    Vector3.Distance(hit.point, player.position) <= 10 && Mathf.Abs(hit.point.y - player.position.y)<5)
                {
                    Debug.Log("Im rahmen");
                    if(targetIsNotGroundOrRightHight)
                    {
                        GetComponent<Renderer>().enabled = true;
                        targetIsNotGroundOrRightHight = false;
                    }

                    transform.position = hit.point;
                }
                else if ((hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("MagicInteractive")) &&
                         Mathf.Abs(hit.point.y - player.position.y) < 5)
                {
                    Debug.Log("Im rahmen aber zu weit weg");
                    if (targetIsNotGroundOrRightHight)
                    {
                        GetComponent<Renderer>().enabled = true;
                        targetIsNotGroundOrRightHight = false;
                    }

                    transform.position = player.position + (hit.point - player.position).normalized * 10;
                }
                else
                {
                    GetComponent<Renderer>().enabled = false;
                    targetIsNotGroundOrRightHight = true;
                }
            }
        }
    }
}
