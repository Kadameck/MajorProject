using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTreesInRange : MonoBehaviour
{
    private List<GameObject> hiddenTrees = new List<GameObject>();

    private void OnTriggerStay(Collider other)
    {
        // Ist das im trigger ein ObstacleTree und ist dieser "unterhalb" des spielers und ist dieser noch nicht unsichtbar
        if(other.gameObject.CompareTag("ObstacleTree") && 
            other.gameObject.transform.position.z < transform.position.z &&
            !hiddenTrees.Contains(other.gameObject))
        {
            hiddenTrees.Add(other.gameObject);
            other.gameObject.GetComponent<Renderer>().enabled = false;
        }
        // ist es ein obstacleBaum und dieser hat einen größeren z wert als der spieler und er ist aber auf der unsichtbar liste
        else if(other.gameObject.CompareTag("ObstacleTree") && 
                 other.gameObject.transform.position.z > transform.position.z && 
                 hiddenTrees.Contains(other.gameObject))
        {
            other.gameObject.GetComponent<Renderer>().enabled = true;
            hiddenTrees.Remove(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ObstacleTree"))
        {
            hiddenTrees.Remove(other.gameObject);
            other.gameObject.GetComponent<Renderer>().enabled = true;
        }
    }
}
