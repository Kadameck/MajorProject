using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEditorViewMask : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject);
    }
}
