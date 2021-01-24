using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAwake : MonoBehaviour
{
    [SerializeField, Tooltip("WolfPlayerFirstMeeting.cs")]
    WolfPlayerFirstMeeting wPFM;
    [SerializeField, Tooltip ("Marker für die Position an die sich der Spieler bewegen soll ehe der Wolf aufwacht")]
    Transform playerPos;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GameObject.FindGameObjectWithTag("Wolf").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Betritt der Spieler den Trigger...
        if(other.gameObject.CompareTag("Player"))
        {
            // Schaut er zu seinem zielort
            other.gameObject.transform.LookAt(playerPos);
            // Werden sämmtliche Bewegungskräfte und Steuerungsmöglichkeiten deaktiviert
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            other.gameObject.GetComponent<ShamanControl>().enabled = false;

            // bewegt den Spieler zur vorgesehenen Position
            StartCoroutine(MovePlayerToPos(other.gameObject));
        }
    }

    /// <summary>
    /// // bewegt den Spieler zur vorgesehenen Position
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    IEnumerator MovePlayerToPos(GameObject player)
    {
        bool moving = true;

        // Loopt so lange, bis der Spieler am gewünschten Platz ist
        while(moving)
        {
            // Prüft ob der Spieler zu weit vom gewünschten Platz weg ist
            if(Vector2.Distance(new Vector2(playerPos.position.x, playerPos.position.z), 
                                new Vector2(player.transform.position.x, player.transform.position.z)) > 0.2f)
            {
                // Bewegt den Spieler richtung gewünschten Platz
                player.transform.Translate(Vector3.forward * Time.deltaTime * 3);
            }
            // ist der Spieler nah genug am gewünschten Platz...
            else
            {
                // Wird er genau auf den gewünschten Platz gesezt und ie While schleife wird beendet
                player.transform.position = playerPos.position;
                player.GetComponent<ShamanControl>().GetAnimator().SetBool("Walk", false);
                player.GetComponent<ShamanControl>().GetAnimator().SetBool("Push", false);
                player.GetComponent<ShamanControl>().GetAnimator().SetBool("Magic", false);
                moving = false;
            }

            // Sorgt dafür das die While schleife nur ein mal pro frame ausgeführt wird
            yield return new WaitForEndOfFrame();
        }

        // Lässt den Spieler zum wolf blicken
        player.transform.LookAt(this.gameObject.transform.parent.transform);
        // Startt das Aufwachen des wolfes
        anim.SetBool("Awake", true);
        
        // Startet alles was nach dem aufwachen passieren soll
        wPFM.WalkToWPOne(anim);
    }
}
