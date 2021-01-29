using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableRock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Prüft ob es der Spieler ist den den Stein berührt
        if(collision.gameObject.CompareTag("Player"))
        {
            Transform player = collision.gameObject.transform;

            if ((player.position.x > transform.position.x && Input.GetAxis("Horizontal") < 0 && (int)Input.GetAxis("Vertical") == 0) || // Spieler rechts neben Stein und geht nach links
                (player.position.x < transform.position.x && Input.GetAxis("Horizontal") > 0 && (int)Input.GetAxis("Vertical") == 0)|| // spieler links neben stein und geht nach rechts
                (player.position.z > transform.position.z && Input.GetAxis("Vertical") < 0 && (int)Input.GetAxis("Horizontal") == 0) || // spieler hinter dem Stein und geht nach "unten"
                (player.position.z < transform.position.z && Input.GetAxis("Vertical") > 0 && (int)Input.GetAxis("Horizontal") == 0)) // Spieler vor dem stein un geht nach "oben"
            {
                player.gameObject.GetComponent<ShamanControl>().SetPushingSomething();
                this.GetComponent<Rigidbody>().isKinematic = false;
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Prüft ob es der Spieler ist den den Stein berührt
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;

            if(player.GetComponent<ShamanControl>().GetPushSomething())
            {
                player.GetComponent<ShamanControl>().SetPushingSomething();
                this.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}
