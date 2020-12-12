using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMagicBehaviour : MonoBehaviour
{
    // Part pieces of the ball movement
    private int points = 50;
    // Part pieces iterator
    private int i = 0;
    // Bool to prevent the ball from already flying with the skill activation click
    private bool ready = false;

    // Waypoints
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;

    /// <summary>
    /// Sets al needed Variables
    /// </summary>
    /// <param name="startPos">ball start position</param>
    /// <param name="targetPos">ball target position (click position)</param>
    /// <param name="distance">distance between start und target position</param>
    public void Throw(Vector3 startPos, Vector3 targetPos, float distance)
    {
        // Sets all necessary Variables
        points = (int)distance;
        p0 = startPos;
        // Calculates a 3rd point so that the ball can fly from the start over it to the target
        p1 = CalculateBowHight(startPos, targetPos, distance);
        p2 = targetPos;

        // Prevent the ball from already flying with the skill activation click
        StartCoroutine(Wait());
    }

    /// <summary>
    /// Berechnet den Hochpunkt des Wurfbogens
    /// </summary>
    /// <param name="startPos">spawnPos</param>
    /// <param name="targetPos">angeklickte stelle</param>
    /// <param name="dist">entfehrnung zwischen start und target</param>
    /// <returns></returns>
    private Vector3 CalculateBowHight(Vector3 startPos, Vector3 targetPos, float dist)
    {
        // Calculates the point exactly in the middle between start and target
        Vector3 halfLength = startPos + (targetPos - startPos).normalized * (dist / 2);
        // Go 10 up from the midpoint to reach the top of the arc of flight
        Vector3 hightPoint = halfLength + new Vector3(0.0f, 10.0f, 0.0f);
        
        return hightPoint;
    }

    /// <summary>
    /// Moves the ball
    /// </summary>
    private void Update()
    {
        // "Bézier curve"
        // B(t) = ((1-t)*(1-t))*startPos + 2*(1-t)*t*bowHight + (t*t)*targetPos

        // Checks if the ball should move
        if (ready)
        {
            // Sets all the variables that are needed for a more clearly calculation of the "Bézier curve"
            float t = i / (float)points;
            float u = 1 - t;
            float uSquare = u * u;
            float tSquare = t * t;

            // This "if" works like a for loop, which repeats its content once per frame 
            // (as long as the condition is correct) instead of as often in a frame as a condition specifies.
            if (i < points + 1)
            {
                // "Bézier curve"
                Vector3 currentPosition = uSquare * p0;
                currentPosition += 2 * u * t * p1;
                currentPosition += tSquare * p2;

                // Sets the ball position to the next point on the "Bézier curve"
                transform.position = currentPosition;
                
                // Increase the iterator
                i++;
            }
            // Destroys the ball if it has reached the target position
            else { Destroy(this.gameObject); }
        }
    }

    /// <summary>
    /// Prevent the ball from already flying with the skill activation click
    /// </summary>
    /// <returns></returns>
    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        ready = true;
    }
}
