using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Climbingskill
/// </summary>
public class ClimbingSpot : MonoBehaviour
{
    [SerializeField, Tooltip(" Key that should move the player upwards")]
    char upKey;
    [SerializeField, Tooltip(" Key that should move the player downwards")]
    char downKey;

    // stores the player if he is climbing
    private GameObject child;

    /// <summary>
    /// Controlls the player movement if he is climbing
    /// </summary>
    private void Update()
    {
        // Checks if the player is climbing
        if (child != null)
        {
            // Checks if the correct Key for climbing up or downward is pressed and moves the player that way 
            if (upKey == 'w' && Input.GetKey(KeyCode.W) ||
               upKey == 'd' && Input.GetKey(KeyCode.D) ||
               upKey == 'a' && Input.GetKey(KeyCode.A))
            {
                    child.transform.Translate(Vector3.up * 0.1f);
            }
            else if (downKey == 'w' && Input.GetKey(KeyCode.S) ||
                    downKey == 'd' && Input.GetKey(KeyCode.D) ||
                    downKey == 'a' && Input.GetKey(KeyCode.A))
            {
                if (!child.GetComponent<ShamanControl>().GetIsGrounded())
                {
                    child.transform.Translate(Vector3.down * 0.1f);
                }
                else
                {
                    SetPlayerFree();
                }
            }
        }
    }

    /// <summary>
    /// Initialised the climbing skill
    /// </summary>
    /// <param name="other">Obj in Trigger</param>
    private void OnCollisionEnter(Collision other)
    {
        // Checks whether the object that enters the trigger is the player, whether the player was climbing just short before and whether he is coming from above or below
        if (other.gameObject.CompareTag("Player")&& other.transform.position.y < transform.position.y)
        {
            // Blocks the normal player movement
            other.gameObject.GetComponent<ShamanControl>().isClimbing = true;
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Prefents the player from falling
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            // Sets the player as child obj from the climbingSpot
            other.transform.SetParent(this.transform);
            // Stores the player to move him in Update()
            child = other.gameObject;
            
        }
    }

    /// <summary>
    /// Resets the player
    /// </summary>
    /// <param name="other">Obj in Target</param>
    private void OnCollisionExit(Collision other)
    {
        // Checks if the obj that is leaving the trigger is the player
        if(other.gameObject.CompareTag("Player"))
        {
            SetPlayerFree();
        }
    }

    private void SetPlayerFree()
    {
        if (child != null)
        {
            child.gameObject.GetComponent<ShamanControl>().isClimbing = false;
            child.transform.SetParent(null);
            child.gameObject.GetComponent<Rigidbody>().useGravity = true;
            // Push the player a little to prevent them from falling back into the trigger
            child.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.up + child.transform.forward) * 1000, ForceMode.Impulse);
            // Reset Variables
            child = null;
        }
    }
}
