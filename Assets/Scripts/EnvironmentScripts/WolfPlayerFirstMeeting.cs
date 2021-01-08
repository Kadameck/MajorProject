using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPlayerFirstMeeting : MonoBehaviour
{
    [SerializeField, Tooltip("Wegpunkt im Gebüsch")]
    Transform wayPoint;
    [SerializeField, Tooltip("Der Spieler")]
    GameObject player;

    private Animator anima;

    // Startet die Lauf animation in richtung spieler (weil der wolf schon so rotiert ist)
    public void WalkToWPOne(Animator anim)
    {
        anima = anim;
        anima.SetBool("Walk", true);
        StartCoroutine(BiteInPlayerDirection());
    }

    // Nähert sich bis auf 7 meter dem spieler und hört dann auf
    IEnumerator BiteInPlayerDirection()
    {
        bool whileLoop = true;

        while(whileLoop)
        {
            // ist der wolf nurnoch 7 meter vor dem spieler...
            if (Vector3.Distance(transform.position, player.transform.position) < 7.0f)
            {
                // Wird die beiß animation ausgeführt und der Loop beendet
                anima.SetBool("Bite", true);
                whileLoop = false;
            }
            yield return new WaitForEndOfFrame();
        }

        // wartet bis die Beisanimation durchgelaufen ist
        yield return new WaitForSeconds(1.7f);
        // rennt ins gebüsch
        RunToWPTwo();
    }

    private void RunToWPTwo()
    {
        // Blickt zum gebüsch
        transform.LookAt(wayPoint.position);
        // rennt los
        anima.SetBool("Run", true);
        StartCoroutine(Vanish());
    }

    // gibt die spielersteuerung wider frei und 
    IEnumerator Vanish()
    {
        // Gibt nach 1 sek die spieler steuerung frei
        yield return new WaitForSeconds(1f);
        player.GetComponent<ShamanControl>().enabled = true;
        
        // stertört den wolf nachdem er eine weitere sekunde weggerannt ist (um sicher zu gehen das der spieler ihn nicht sehen kann)
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
