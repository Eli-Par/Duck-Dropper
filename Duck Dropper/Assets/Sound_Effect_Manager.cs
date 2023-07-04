using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Effect_Manager : MonoBehaviour
{

    [SerializeField] private float delayBetweenSounds = 0.1f;

    float timeSinceSound = 0;

    Duck_Sounds bestSounds = default;
    float bestVolume = 0;

    bool receivedRequest = false;

    [SerializeField] private int maxConcurrentSounds = default;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug code to change the framerate on the fly to test in lag conditions
        if(Input.GetKeyDown("y"))
        {
            Application.targetFrameRate = -1;
            Debug.Log("Framerate Unlimited");
        }
        if (Input.GetKeyDown("u"))
        {
            Application.targetFrameRate = 40;
            Debug.Log("Framerate 40");
        }
        if (Input.GetKeyDown("i"))
        {
            Application.targetFrameRate = 30;
            Debug.Log("Framerate 30");
        }
        if (Input.GetKeyDown("o"))
        {
            Application.targetFrameRate = 20;
            Debug.Log("Framerate 20");
        }
        if (Input.GetKeyDown("p"))
        {
            Application.targetFrameRate = 15;
            Debug.Log("Framerate 15");
        }
        //End debug code

        //If no request has been received, set the time since a sound played to half the delay
        if(!receivedRequest)
        {
            timeSinceSound = delayBetweenSounds / 2;
        }

        //Check if the time since a sound played is more than the delay and there is a sound to play
        if (timeSinceSound >= delayBetweenSounds && bestSounds != null)
        {
            //Play 1 or more sounds up to a maximum based on how much time has passed. If more than 1 sound would of played in the time interval, play multiple
            for(int i = 0; i < Mathf.Max(1, (int) Mathf.Min(maxConcurrentSounds, Mathf.Floor(timeSinceSound / delayBetweenSounds))); i++)
            {
                //Play a hit sound on the best duck
                bestSounds.PlayHit(bestVolume);
            }

            //Reset the best sound and best volume to be nothing
            bestSounds = null;
            bestVolume = 0;

            //Reset the time since a sound played to 0
            timeSinceSound = 0;
        }

        timeSinceSound += Time.deltaTime;
    }

    public void RequestHitSound(Duck_Sounds duckSounds, float volume)
    {
        //Mark that a request has been received
        receivedRequest = true;

        //If the volume of the request is larger than the best volume so far, this hit sound is the new best
        if (volume > bestVolume)
        {
            bestVolume = volume;
            bestSounds = duckSounds;
        }
    }
}
