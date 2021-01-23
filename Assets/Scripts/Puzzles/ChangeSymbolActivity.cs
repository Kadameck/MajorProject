using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSymbolActivity : MonoBehaviour
{
    [SerializeField]
    NewLightstone corespondingLightstone;
    [SerializeField]
    GameObject symbol;
    [SerializeField]
    GameObject SymbolPointLight;

    private Color activeColor;
    private bool active = false;
    private Material mat;

    void Start()
    {
        mat = symbol.GetComponent<Renderer>().material;
        activeColor = mat.color;
        mat.color = Color.black;
        mat.DisableKeyword("_EMISSION");
        SymbolPointLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(corespondingLightstone.GetIsActive() && !active)
        {
            mat.color = activeColor;
            mat.EnableKeyword("_EMISSION");
            SymbolPointLight.SetActive(true);
            active = true;
        }
        else if(!corespondingLightstone.GetIsActive() && active)
        {
            mat.color = Color.black;
            mat.DisableKeyword("_EMISSION");
            SymbolPointLight.SetActive(false);
            active = false;
        }
    }
}
