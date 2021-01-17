using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFadeLayer : MonoBehaviour
{
    [SerializeField]
    GameObject orb;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForDestroy());
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(2);
        orb.GetComponent<OrbFollowPath>().SetFadeIsDestroyed();
        
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }
}
