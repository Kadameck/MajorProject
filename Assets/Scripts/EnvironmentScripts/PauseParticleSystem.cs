using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseParticleSystem : MonoBehaviour
{
    [SerializeField]
    ParticleSystem system;
    [SerializeField]
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PauseTimer());
    }

    private IEnumerator PauseTimer()
    {
        float delayTimer = 0;

        while(delayTimer <= timer)
        {
            delayTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        system.Pause();
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject.GetComponent<PauseParticleSystem>());
    }
}
