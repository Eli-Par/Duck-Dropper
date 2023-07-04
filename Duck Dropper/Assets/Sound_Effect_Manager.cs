using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Effect_Manager : MonoBehaviour
{

    public float delayBetweenSounds = 0.1f;

    float timeSinceSound = 0;

    Duck_Sounds bestSounds = default;
    float bestVolume = 0;

    public int arraySize = 10;
    Duck_Sounds[] usableSounds;
    float[] volumes;

    int filledIndex = 0;

    int requests = 0;

    // Start is called before the first frame update
    void Start()
    {
        usableSounds = new Duck_Sounds[arraySize];
        volumes = new float[arraySize];
    }

    // Update is called once per frame
    void Update()
    {
        while(timeSinceSound >= delayBetweenSounds && usableSounds[0] != null)
        {
            usableSounds[0].PlayHit(volumes[0]);

            usableSounds[Mathf.Min(arraySize - 1, filledIndex)] = null;
            volumes[Mathf.Min(arraySize - 1, filledIndex)] = 0;

            timeSinceSound -= delayBetweenSounds;
            filledIndex--;

            if (filledIndex < 0)
            {
                filledIndex = 0;
            }

            //Debug.Log(volumes[0]);

            //Debug.Log(requests);
            requests = 0;
        }

        if(timeSinceSound >= delayBetweenSounds && usableSounds[0] == null)
        {
            Debug.Log("None queued!");
        }

        timeSinceSound += Time.deltaTime;
    }

    public void RequestHitSound(Duck_Sounds duckSounds, float volume)
    {
        if(filledIndex < 0)
        {
            filledIndex = 0;
        }

        requests++;
        if(volume > volumes[Mathf.Min(arraySize - 1, filledIndex)])
        {
            AddSound(duckSounds, volume);
        }
    }

    void AddSound(Duck_Sounds duckSounds, float volume)
    {
        if(filledIndex < arraySize)
        {
            usableSounds[filledIndex] = duckSounds;
            volumes[filledIndex] = volume;

            RearrangeFromIndex(filledIndex);

            filledIndex++;
        }
        else
        {
            usableSounds[arraySize - 1] = duckSounds;
            volumes[arraySize - 1] = volume;
        }
    }

    void RearrangeFromIndex(int k)
    {
        while (k > 0 && volumes[k - 1] > volumes[k])
        {
            Swap(k);
        }
    }

    void Swap(int k)
    {
        Duck_Sounds temp = usableSounds[k];
        usableSounds[k] = usableSounds[k - 1];
        usableSounds[k - 1] = temp;

        float temp2 = volumes[k];
        volumes[k] = volumes[k - 1];
        volumes[k - 1] = temp2;
    }
}
