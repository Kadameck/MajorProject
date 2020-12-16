using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Every object that contains this script will be carryable
/// </summary>
public class CarryObject : MonoBehaviour
{
    private GameObject normalParent;
    private CarrySphere cS;
    private GameObject cSphere;
   // private Transform player;
    private bool isCarried = false;
    private bool willBePlaced = false;

    // Start is called before the first frame update
    void Start()
    {
        normalParent = GameObject.FindGameObjectWithTag("TerrainHolder");
    }

    private void Update()
    {
        if(isCarried || willBePlaced)
        {
            transform.position = cSphere.transform.position;
            
            if(willBePlaced)
            {
                PutDown();
            }
        }
    }
    public void Take(GameObject carrySphere)
    {
        cSphere = Instantiate(carrySphere, transform.position, Quaternion.identity);
        GetComponent<Rigidbody>().useGravity = false;
        this.transform.SetParent(cSphere.transform);
        isCarried = true;
    }

    public void PutDown(Vector3 place)
    {
        willBePlaced = true;
        cS = cSphere.GetComponent<CarrySphere>();
        cS.SetPutDownPlace(place + Vector3.up);
    }

    private void PutDown()
    {
        if (cS.GetTargetReached())
        {
            DestroyCarrySphere();
        }
    }

    private void DestroyCarrySphere()
    {
        // Prüft ob das normale parentobjekt gerade aktiv ist und legt es nur dann wider als parent fest
        // sonst könnte man das objekt in der höhle ablegen und es würde sofort verschwinden, da es ein 
        // child des terrains wird, welches ja inaktiv ist, wenn man in der höhle ist
        // dadurch wäre das getragene objekt sofort ebenfalls deaktiv und würde erst wider sichtbar (und damit aufhebbar) sein, wenn
        // das Terrain sichtbar wird (also beim verlassen der höhle) da kann es aber passieren, dass das objekt so weit in der höhle drinnen liegt, dass
        // man es jetzt nicht mehr nehmen kann weil es ja jedesmal wenn man die höhle betritt mit dem terrain inaktiv wird.
        if(normalParent.activeSelf)
        {
            this.transform.SetParent(normalParent.transform);
        }
        else
        {
            // Ist das terrain gerade inaktiv (der spieler also in einer höhle) bekommt das getragene objekt einfach keinen parent beim ablegen
            // dadurch das die höhle ja nicht verschwindet wenn man sie verlässt fällt das objekt auch nicht ins nichts
            // beim terrain muss es aber ein child sein sonst würde genau das passieren wenn das terrain beim betreten der höhle inaktiv wird und damit der collider
            // fehlt, auf dem das getragene objekt abgelegt wurde. das objekt würde also ins nichts fallen wenn es kein childobjekt des terrains wäre sobalt der spieler die höhle betritt und
            // das objekt aber ihrgendwo liegt
            this.transform.SetParent(null);
        }
        GetComponent<Rigidbody>().useGravity = true;
        isCarried = false;
        willBePlaced = false;
        Destroy(cSphere.gameObject);
    }
}
