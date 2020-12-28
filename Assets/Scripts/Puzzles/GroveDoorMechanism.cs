using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Jedesmal wenn ein lichtstein aktiviert wird, erhöt sich der counter um 1.
 * wenn der counter die gleiche zahl hat wie die angegebene activ lightstone number dann sind gesuo viele lichtsteine aktiv wie es sein sollen
 * wenn dies der fall ist wird die öffnungsanimation gestartet
 * 
 * wenn ein lichtstein wieder aus geht wird der counter um eins verringert
 * sollte dies passieren und die door ist offen, wird die öffnungsanimation rückwärts abgespielt
 * */

public class GroveDoorMechanism : MonoBehaviour
{
    [SerializeField, Tooltip("How many Lightstones have to be activ to open the door?")]
    int activLightstoneNumber;

    private Animator anim;
    private int activeLightstoneCounter;
    private bool doorIsOpen = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Wird aktiviert wenn ein ligtstein aktiviert wird
    public void IncreaseActiveLightstoneCounter()
    {
        activeLightstoneCounter++;
        CheckForOpeningAndClosing();
    }

    // Prüft ob die tür auf oder zu gehen soll
    private void CheckForOpeningAndClosing()
    {
        // Wenn genauso viele steine aktiv sidn wie es sein sollen und die tür nicht schon offen ist
        if(activeLightstoneCounter == activLightstoneNumber && !doorIsOpen)
        {
            OpenTheDoor();
        }
        // Wenn weniger steine aktiv sind als es sein sollen aber die tür offen ist
        else if(activeLightstoneCounter < activLightstoneNumber && doorIsOpen)
        {
            CloseTheDoor();
        }
    }

    // Startet die öffnungs animation
    private void OpenTheDoor()
    {
        StartCoroutine(WaitWithOpenInFrameOne());
        doorIsOpen = true;
    }

    // wenn bereits zu beginn genug lichtsteine aktiv sind kann es zu fehlern kommen wenn mit der öffnungsanimation nicht bis zum ende des frames gewartet wird
    IEnumerator WaitWithOpenInFrameOne()
    {
        yield return new WaitForEndOfFrame();
        // gibt an das die animation mit speed 1 abgespielt werden soll (normales tempo und forwärts)
        anim.SetFloat("Direction", 1.0f);
        anim.SetBool("Opening", true);
    }

    // Wird aktiviert wenn ein ligtstein Deaktiviert wird
    public void DegreaseActiveLightstoneCounter()
    {
        activeLightstoneCounter--;
        CheckForOpeningAndClosing();
    }

    // startet die schließungs animation
    private void CloseTheDoor()
    {
        // gibt an das die animation mit speed -1 abgespielt werden soll (normales tempo und rückwärts)
        anim.SetFloat("Direction", -1.0f);
        anim.SetBool("Opening", false);

        doorIsOpen = false;
    }
}
