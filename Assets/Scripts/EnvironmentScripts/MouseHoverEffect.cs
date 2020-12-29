using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverEffect : MonoBehaviour
{
    private Material mat;
    private Shader normalShader;
    public Material MaterialWithStandartShader;
    private Shader transShader;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        normalShader = mat.shader;
        transShader = MaterialWithStandartShader.shader;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        mat.shader = transShader;
        Color oldColor = mat.color;
        Color NewColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);
        mat.SetColor("_Color", NewColor);

       // mat.EnableKeyword("_EMISSION");
    }

    private void OnMouseExit()
    {
        mat.shader = normalShader;
       // mat.DisableKeyword("_EMISSION");
    }
}
