using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Effect_Manager : MonoBehaviour
{

    public float delayBetweenSounds = 0.1f;

    float timeSinceSound = 0;

    Duck_Sounds bestSounds = default;
    float bestVolume = 0;

    int requests = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeSinceSound >= delayBetweenSounds && bestSounds != null)
        {
            bestSounds.PlayHit(bestVolume);

            bestSounds = null;
            bestVolume = 0;

            timeSinceSound = 0;

            //Debug.Log(requests);
            requests = 0;
        }

        timeSinceSound += Time.deltaTime;
    }

    public void RequestHitSound(Duck_Sounds duckSounds, float volume)
    {
        requests++;
        if (volume > bestVolume)
        {
            bestVolume = volume;
            bestSounds = duckSounds;
        }
    }
}
