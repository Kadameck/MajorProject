using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrovePortalOpening : MonoBehaviour
{
    [SerializeField]
    GameObject mechanism;
    
    private StonePortalMechanism mechScript;
    private Animator anim;
    private bool beschaftigt;

    // Start is called before the first frame update
    void Start()
    {
        mechScript = mechanism.GetComponent<StonePortalMechanism>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mechScript.opening && !beschaftigt)
        {
            beschaftigt = true;
            anim.SetBool("Open", true);
        }
    }
}
