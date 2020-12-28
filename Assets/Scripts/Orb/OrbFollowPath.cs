using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbFollowPath : MonoBehaviour
{
    [SerializeField, Tooltip("Specifies how long the orb should wait for the player before he leaves the path to follow the player (Seconds)")]
    float waitingTimer = 10;
    // Die Objekte die die Waypoints enthalten
    //[SerializeField, Tooltip("The waypoint containers that together make up the desired path. Each container consists of 4 waypoints")]
    //GameObject[] pathContainer; // routes

    public GameObject[] stages;

    private List<GameObject> pathContainer = new List<GameObject>();

    private OrbStageTransition oST;
    private Transform player;
    private int iterator = 0;
    private List<Vector3> positions = new List<Vector3>();
    private List<Vector3> gizmoPositions = new List<Vector3>();
    private Vector3 gizmoPos;
    private float t;
    private Vector3 orbPos;
    private float speed;
    private bool canStartNewCoroutine;
    private int c;
    private Coroutine wFPCoroutine;
    private bool followPlayer = false;
    private Vector3 lastPathPosition;
    private bool stageFinished = false;
    private bool transitionHasStarted = false;
    private int stageNumb;

    // Start is called before the first frame update
    void Start()
    {
        oST = GetComponent<OrbStageTransition>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        t = 0;
        speed = 0.5f;
        canStartNewCoroutine = true;
        stageNumb = 0;

        InitStage(stageNumb);
    }

    /// <summary>
    /// Erfasst die punkte dir für die berechnung der nun zu folgenden stage benötigt werden und speichert diese in der richtigen reihenfolge in einer liste
    /// </summary>
    /// <param name="stageNumber">Die stage der gefolgt werden soll (0= die erste, 1= die zweite, usw</param>
    private void InitStage(int stageNumber)
    {
        pathContainer.Clear();
        positions.Clear();

        // Loopt durch die child (also container) des ersten stage
        for (int stageChilds = 0; stageChilds < stages[stageNumber].transform.childCount; stageChilds++)
        {
            pathContainer.Add(stages[stageNumber].transform.GetChild(stageChilds).gameObject);
        }

        // Loopt durch die container
        for (int container = 0; container < pathContainer.Count; container++)
        {
            // Loopt durch die Childobjekte des pathcontainers
            for (int waypoint = 0; waypoint < pathContainer[container].transform.childCount; waypoint++)
            {
                positions.Add(pathContainer[container].transform.GetChild(waypoint).position);
            }
        }

        transform.position = positions[0];
    }

    private void Update()
    {
        if(stageFinished && !transitionHasStarted) { StartStageTransition(); }
        // if(followPlayer) { FollowPlayer(); }
        // if (canStartNewCoroutine && !followPlayer && !stageFinished) { StartCoroutine(FollowPath()); }
        if (canStartNewCoroutine && !stageFinished) { StartCoroutine(FollowPath()); }
    }

    /// <summary>
    /// Der algorythmus der dafür sorgt das der orb allen bezier kurven der aktuellen stage nach einander folgt
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        // Prüft ob der Spieler na genug am orb ist damit der orb weiter dem Pfad folgen kann
        if (CloseEnoughCheck(transform.position, player.position, 10.0f))
        {
            if(wFPCoroutine != null)
            {
                StopCoroutine(wFPCoroutine);
                wFPCoroutine = null;
            }

            canStartNewCoroutine = !canStartNewCoroutine;

            Vector3 p0 = positions[iterator + 0];
            Vector3 p1 = positions[iterator + 1];
            Vector3 p2 = positions[iterator + 2];
            Vector3 p3 = positions[iterator + 3];

            iterator += 4;

            while (t < 1)
            {
                t += Time.deltaTime * speed;

                // Bézier Curve
                orbPos = Mathf.Pow(1 - t, 3) * p0 +
                           3 * Mathf.Pow(1 - t, 2) * t * p1 +
                           3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                           Mathf.Pow(t, 3) * p3;

                transform.position = orbPos;
                yield return new WaitForEndOfFrame();
            }

            t = 0f;

            if (iterator + 3 > positions.Count - 1)
            {
                SetStageFinished();
                iterator = 0;
            }

            canStartNewCoroutine = !canStartNewCoroutine;
        }
        else
        {
            if(wFPCoroutine == null)
            {
                wFPCoroutine = StartCoroutine(WaitForPlayer());
            }
        }
    }

    /// <summary>
    /// Berechnet die Distanz zwischen den beiden angegebenen punkten und gibt nur dann true zurück wenn der angegebene maximalwert eingehalten wird
    /// </summary>
    /// <returns>Is the player close enough to the orb?</returns>
    private bool CloseEnoughCheck(Vector3 controllPosA, Vector3 controllPosB, float maxDistance)
    {
        if(Vector3.Distance(controllPosA, controllPosB) <= maxDistance)
        {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Wartet auf den spieler
    /// Wird abgebrochen wenn der spieler kommt oder leitet die spieler verfolgung ein wenn der spieler zu lange weg bleibt
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForPlayer()
    {
        lastPathPosition = transform.position;
        yield return new WaitForSeconds(waitingTimer);
        followPlayer = true;
        wFPCoroutine = null;
    }

    /// <summary>
    /// Verfolgt den spieler
    /// </summary>
  //  private void FollowPlayer()
  //  {
  //      // Prüft ob der spieler zu weit vom orb weg ist
  //      if(!CloseEnoughCheck(transform.position, player.position, 10.0f))
  //      {
  //          // Wenn der spieler zu weit vom orb weg ist, folgt der orb dem spieler
  //          transform.LookAt(player.position);
  //          transform.Translate(Vector3.forward * Time.deltaTime * (speed * 10));
  //      }
  //      // Wenn der spieler nah genug am orb ist...
  //      else
  //      {
  //          // Wenn der spieler nah genug am orb ist aber der orb nicht nah genug an seiner letzten pfadposition
  //          if(!CloseEnoughCheck(lastPathPosition, transform.position, 0.1f))
  //          {
  //              transform.LookAt(lastPathPosition);
  //              // dann bewegt sich der orb zu seiner letzten pfadposition
  //              transform.Translate(Vector3.forward * Time.deltaTime * (speed*10));
  //          }
  //          // ist der spieler nah genug am orb und der orb selbst (und somit auch der spieler) nah genug an der letzten pfadposition
  //          else
  //          {
  //              followPlayer = false;
  //          }
  //      }
  //  }
    
    /// <summary>
    /// Welchselt zwischen stage ist beendet und stage ist nicht beendet (für die übergänge zwischen den stages)
    /// </summary>
    private void SetStageFinished()
    {
        stageFinished = !stageFinished;
    }

    /// <summary>
    /// Startet das verhalten das der orb jeweils zwischen den stages haben soll (zB beim lichtstein soll der orb den spieler darauf hinweisen dass er den stein aktivieren kann ehe
    /// er (der orb) weiter zur nächsten stage geht)
    /// </summary>
    private void StartStageTransition()
    {
        transitionHasStarted = true;
        oST.StartTransition(stageNumb);
    }
    
    /// <summary>
    /// Startet die nächste stage und setzt entsprechend alle variablen zurück die dafür gebraucht werden
    /// </summary>
    public void StartNextStage()
    {
        SetStageFinished();
        followPlayer = false;
        transitionHasStarted = false;
        stageNumb++;
        InitStage(stageNumb);
    }
    private void OnDrawGizmos()
    {
        gizmoPositions.Clear();
        c = 0;
   
        if (pathContainer.Count == 0)
        {
            // Loopt durch die child (also container) des ersten stage
            for (int stageChilds = 0; stageChilds < stages[0].transform.childCount; stageChilds++)
            {
                pathContainer.Add(stages[3].transform.GetChild(stageChilds).gameObject);
            }
        }
   
        for (int container = 0; container < pathContainer.Count; container++)
        {
            // Loopt durch die Childobjekte des pathcontainers
            for (int waypoint = 0; waypoint < pathContainer[container].transform.childCount; waypoint++)
            {
                gizmoPositions.Add(pathContainer[container].transform.GetChild(waypoint).position);
            }
   
            for (float i = 0; i <= 1; i += 0.05f)
            {
                gizmoPos = Mathf.Pow(1 - i, 3) * gizmoPositions[c] +
                           3 * Mathf.Pow(1 - i, 2) * i * gizmoPositions[c + 1] +
                           3 * (1 - i) * Mathf.Pow(i, 2) * gizmoPositions[c + 2] +
                           Mathf.Pow(i, 3) * gizmoPositions[c + 3];
   
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(gizmoPos, 0.25f);
   
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gizmoPositions[c], gizmoPositions[c + 1]);
            Gizmos.DrawLine(gizmoPositions[c + 2], gizmoPositions[c+ 3]);
   
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(gizmoPositions[c + 1], 0.75f);
            Gizmos.DrawSphere(gizmoPositions[c + 2], 0.75f);
            
            c += 4;
        }
    }
}