using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfFollowPlayer : MonoBehaviour
{
    // Das Objekt dem gefolgt werden soll
    [SerializeField]
    GameObject target;
    // Ab welchen Abstand soll der Follower stehen bleiben sich weiter dem Target zu nähern
    [SerializeField]
    float minDistance = 5;
    // Wie schnell bewegt sich der Follower
    [SerializeField]
    float speed;

    // Die momentane entfehrnung zwischen follower und target
    private float targetDistance;
    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        // Dreht den Follower so dass er zum spieler sieht
        transform.LookAt(target.transform.position);

        targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance >= minDistance)
        {
            anim.SetFloat("RunSpeed", Time.deltaTime * speed);
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }
    }
}
