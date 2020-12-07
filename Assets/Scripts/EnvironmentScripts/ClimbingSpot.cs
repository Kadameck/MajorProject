using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingSpot : MonoBehaviour
{
    [SerializeField]
    char upKey;
    [SerializeField]
    char downKey;

    private GameObject child;
    private bool memoryPlayer = false;

    // Start is called before the first frame update

    private void Update()
    {
        if (child != null)
        {
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
                child.transform.Translate(Vector3.down * 0.1f);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && memoryPlayer == false && other.transform.position.y < transform.position.y)
        {
            other.GetComponent<ShamanControl>().isClimbing = true;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponent<Rigidbody>().useGravity = false;
            other.transform.SetParent(this.transform);
            child = other.gameObject;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<ShamanControl>().isClimbing = false;
            other.transform.SetParent(null);
            other.GetComponent<Rigidbody>().useGravity = true;
            other.GetComponent<Rigidbody>().AddForce((Vector3.up + other.transform.forward) * 1000, ForceMode.Impulse);
            child = null;
            memoryPlayer = true;
            StartCoroutine(test());
        }
    }


    IEnumerator test()
    {
        yield return new WaitForSeconds(0.5f);
        memoryPlayer = false;
        
    }
}
