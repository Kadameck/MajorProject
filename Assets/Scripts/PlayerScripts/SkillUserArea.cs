using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUserArea : MonoBehaviour
{
    private ShamanControl sCon;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        sCon = FindObjectOfType<ShamanControl>();
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sCon.UseMagic() && rend.enabled == false)
        {
            rend.enabled = true;
        }
        else if (!sCon.UseMagic() && rend.enabled == true)
        {
            rend.enabled = false;
        }
    }
}
