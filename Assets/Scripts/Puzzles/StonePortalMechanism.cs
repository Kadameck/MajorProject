using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePortalMechanism : MonoBehaviour
{
    [SerializeField]
    GameObject PortalDoor;

    [SerializeField]
    GameObject[] activationPrismsOrLightstones;

    private bool[] prismStates;
    private bool solved = false;
    private Animator anim;

    private void Start()
    {
        prismStates = new bool[activationPrismsOrLightstones.Length];
        anim = PortalDoor.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(solved && PortalDoor != null)
        {
            OpenPortal();
        }
    }

    public void CheckNumberOfLightbeams(GameObject prismOrLightstone, bool state)
    {
        int index = 0;
        // geht duch alle angegebenen prismen oder steine
        foreach(GameObject go in activationPrismsOrLightstones)
        {
            // Wenn der stein der diese funktion hier aufgerufen hat der gleiche ist wie der der gerade in der liste geprüft wird...
            if(prismOrLightstone == go)
            {
                // Wenn für diesen stein bereits der richtige state drin steht
                if(prismStates[index] == state)
                {
                    // schleife abbrechen
                    break;
                }
                // wenn für diesen stein ein falscher state drinn steht
                else
                {
                    // ändere den state
                    prismStates[index] = state;
                    // Immer wenn ein state sich geändert hat soll geprüft werden ob jetzt alle stats richtig sind
                    if(CheckForSolution())
                    {
                        solved = true;
                    }
                }
            }

            // Erhöhe den index damit die positionen der objekte im gameobject array auf ihre passenden positionen im bool aray gemappt werden
            index++;
        }
    }

    /// <summary>
    /// Prüft ob alle lichter an sind
    /// </summary>
    /// <returns></returns>
    private bool CheckForSolution()
    {
        // Soolt durch die bool array elemente
        for (int i = 0; i < prismStates.Length-1; i++)
        {
            // prüft für jedes element ob es true ist
            if(prismStates[i] == true)
            {
                // ist es true soll das nächste geprüft werden
                continue;
            }
            else
            {
                // ist es false kann die schleife beendet werden denn es sind nicht alle beams aktiv
                return false;
            }
        }

        // Hier kommt man nur an wenn alle lichtstrahlen aktiv sind
        return true;
    }

    private void OpenPortal()
    {
        Debug.Log("bahdjkahdjkha");
        anim.SetBool("open", true);
        //Destroy(PortalDoor.gameObject);
        //PortalDoor = null;
    }
}
