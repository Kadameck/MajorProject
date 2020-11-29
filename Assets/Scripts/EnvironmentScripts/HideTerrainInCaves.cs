using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTerrainInCaves : MonoBehaviour
{
    private Terrain terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain = FindObjectOfType<Terrain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ChangeTerrainVisibility());
        }
    }

    private IEnumerator ChangeTerrainVisibility()
    {
        yield return new WaitForFixedUpdate();

        terrain.enabled = !terrain.enabled;
    }
}
