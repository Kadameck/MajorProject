using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfFollowPlayer : MonoBehaviour
{

    private Transform player;
    private Animator anim;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.position, transform.position) > 5f)
        {
            transform.LookAt(player.position);
            transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime * 3);
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }
    }
}
