using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroveFadeIn : MonoBehaviour
{
    [SerializeField]
    AnimationClip clip;

    [SerializeField]
    Animator anim;
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            anim.SetBool("isActive", true);
            StartCoroutine(DestroyAfterAnim());
        }
    }

    IEnumerator DestroyAfterAnim()
    {
        yield return new WaitForSeconds(clip.length);

        Destroy(this.gameObject);
    }
}
