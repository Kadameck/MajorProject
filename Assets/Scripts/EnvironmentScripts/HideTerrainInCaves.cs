using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ensures that the terrain will be set to invisible when entering a cave and visible when leaving a cave
/// </summary>
public class HideTerrainInCaves : MonoBehaviour
{
    // Gameobject that contains the Terrain
    private GameObject terrainHolder;

    // Start is called before the first frame update
    void Start()
    {
        // Finds the gameobject that contains the Terrain
        terrainHolder = GameObject.FindGameObjectWithTag("TerrainHolder");
    }

    /// <summary>
    /// Handles Collision enters
    /// </summary>
    /// <param name="other">Object  that enters the Collider</param>
    private void OnTriggerEnter(Collider other)
    {
        // Checks if it is the player that is entering the Trigger
        if(other.gameObject.CompareTag("Player"))
        {
            // Calls the Coroutine that will be disable or enable the Terrain
            StartCoroutine(ChangeTerrainVisibility());
        }
    }

    /// <summary>
    /// Disable or enable the terrain
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeTerrainVisibility()
    {
        // Wait until the next "fixed update" to avoid errors
        yield return new WaitForFixedUpdate();

        // Deactivates the object that contains the terrain
        terrainHolder.SetActive(false);
    }
}
