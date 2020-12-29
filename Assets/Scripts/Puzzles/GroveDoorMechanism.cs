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
    [SerializeField]
    GameObject portablePrism;

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

    private void Update()
    {
        // prüft ob alle lichtsteine an sind und die tür aber noch zu ist
        // wenn dieser umstand der fall ist, dann wird jeden frame in der CheckForOpeningAndClosing() geprüft ob das
        // bewegliche prisma plaziert wurde
        if (activeLightstoneCounter == activLightstoneNumber && !doorIsOpen)
        {
            CheckForOpeningAndClosing();
        }
    }

    // Prüft ob die tür auf oder zu gehen soll
    // wird jedes mal aufgerufen wenn sich der zustand eines lichtsteins verändert
    // oder jeden frame sobalt genausoviele lichtsteine aktiv sind wie es sein soll und die tür noch zu ist
    // der grund ist einfach der, das ein prisma prisma erst richtig positioniert werden muss
    // d.h selbst wenn alle lichtsteine an sind bedeutet das nicht das die tür aufgehen soll
    private void CheckForOpeningAndClosing()
    {
        // Wenn genauso viele steine aktiv sidn wie es sein sollen und die tür nicht schon offen ist
        if(activeLightstoneCounter == activLightstoneNumber && !doorIsOpen && portablePrism.GetComponent<Prism>().GetIsPlaced())
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
