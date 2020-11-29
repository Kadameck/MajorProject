using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The magic skill projectile
/// </summary>
public class MagicSkill : MonoBehaviour
{
    // Flying speed
    private float speed = 10;

    // Flying Direction
    private Vector3 direction;
    // Click Position
    private Vector3 desPos;

    /// <summary>
    /// Moves the Effect
    /// </summary>
    private void Update()
    {
        // Moves the effect in the destinated direction with the given speed
        transform.Translate(direction * speed * Time.deltaTime);
 
        Destroy();
    }
    
    /// <summary>
    /// Calculates the wanted flying direction and stores the destinated position
    /// </summary>
    /// <param name="startPos">Players Hand position</param>
    /// <param name="destinationPos">Click Position</param>
    public void Fly(Vector3 startPos, Vector3 destinationPos)
    {
        // Callvulates a vector from the start to the click psoition
        direction = (destinationPos - startPos).normalized;
        //Stores the click Position 
        desPos = destinationPos;
    }

    /// <summary>
    /// Destroys the effekt if it is at its destination position
    /// </summary>
    private void Destroy()
    {
        // Checks if the destination position is reached
        if(Vector3.Distance(transform.position, desPos) <= 0.2f)
        {
            // Destroys this effekt GameObject
            Destroy(this.gameObject);
        }
    }

}
