using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck_Sounds : MonoBehaviour
{
    [SerializeField] private Rigidbody rb = default;

    [SerializeField] private AudioSource[] sources = default;

    [SerializeField] private float relativeVelCutoff = 0.6f;
    [SerializeField] private float velCutoff = 0.5f;

    [SerializeField] private float otherDuckSoundCutoff = 0.1f;

    [SerializeField] private float velocityDivisor = 5f;
    [SerializeField] private float maxVolume = 0.5f;
    [SerializeField] private float importantMaxVolume = 1f;
    [SerializeField] private float pitchShiftMax = 0.1f;

    [SerializeField] private float delayBetweenSounds = 0.2f;

    [SerializeField] private float yCutoff = 6f;

    [Space]
    public float timeSinceSound = 10;

    public bool isImportant = false;

    Sound_Effect_Manager soundEffectManager;

    private float volumeMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        //Get a reference to the sound effect manager that controls which ducks are allowed to play a hit sound
        soundEffectManager = (Sound_Effect_Manager)FindObjectOfType(typeof(Sound_Effect_Manager));

        volumeMultiplier = soundEffectManager.volumeMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSound += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if the relative velocity is high enough for a hit sound
        if(collision.relativeVelocity.magnitude >= relativeVelCutoff)
        {
            //Check if the duck is moving enough for a hit sound and the y position is at or below the y cutoff
            if(rb.velocity.magnitude >= velCutoff && transform.position.y <= yCutoff)
            {
                //Check if the other duck is not playing a sound
                Duck_Sounds otherDuckSounds = collision.gameObject.GetComponent<Duck_Sounds>();
                if (otherDuckSounds == null || otherDuckSounds.timeSinceSound > otherDuckSoundCutoff)
                {
                    //Check if enough time has passed since the last sound effect played
                    if(timeSinceSound >= delayBetweenSounds)
                    {
                        //Check if this duck is important or not. If it is important it bypasses the sound effect manager so it always plays a sound
                        if(isImportant)
                        {
                            //Calculate the volume of the hit sound based on the speed of impact using the important duck values
                            float volume = Mathf.Min(importantMaxVolume, collision.relativeVelocity.magnitude / velocityDivisor);

                            //Play a random hit sound at that volume
                            PlayHit(volume);
                        }
                        else
                        {
                            //Calculate the volume of the hit sound based on the speed of impact using the regular duck values
                            float volume = Mathf.Min(maxVolume, collision.relativeVelocity.magnitude / velocityDivisor);

                            //Request that the sound effect manager plays a hit sound at this duck with that volume
                            soundEffectManager.RequestHitSound(this, volume);
                        }
                        
                    }

                    
                }
            }
        }
    }

    public void PlayHit(float volume)
    {
        //Pick a random audio source to play the hit sound using
        AudioSource source = sources[Random.Range(0, sources.Length)];

        //Randomize the pitch within a range
        source.pitch = 1 + Random.Range(-pitchShiftMax, pitchShiftMax);

        //Set the volume and play the sound
        source.volume = volume * volumeMultiplier;
        source.Play();

        //Reset the amount of time since a hit sound played
        timeSinceSound = 0;

    }
}
