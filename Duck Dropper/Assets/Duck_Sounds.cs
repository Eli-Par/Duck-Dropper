using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck_Sounds : MonoBehaviour
{
    public Rigidbody rb = default;

    public AudioSource[] sources = default;

    public float relativeVelCutoff = 0.6f;
    public float velCutoff = 0.5f;

    public float otherDuckSoundCutoff = 0.1f;

    public float velocityDivisor = 5f;
    public float maxVolume = 0.5f;
    public float importantMaxVolume = 1f;
    public float pitchShiftMax = 0.1f;

    public float delayBetweenSounds = 0.2f;

    [Space]
    public float timeSinceSound = 10;

    GameObject lastCollision = null;

    int collisionCount = 0;

    public bool isImportant = false;

    Sound_Effect_Manager soundEffectManager;

    // Start is called before the first frame update
    void Start()
    {
        soundEffectManager = (Sound_Effect_Manager)FindObjectOfType(typeof(Sound_Effect_Manager));
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSound += Time.deltaTime;

        if(collisionCount > 1) Debug.Log(collisionCount);
        collisionCount = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude >= relativeVelCutoff)
        {
            if(rb.velocity.magnitude >= velCutoff)
            {
                Duck_Sounds otherDuckSounds = collision.gameObject.GetComponent<Duck_Sounds>();
                if (otherDuckSounds == null || otherDuckSounds.timeSinceSound > otherDuckSoundCutoff)
                {
                    if(lastCollision != collision.gameObject && timeSinceSound >= delayBetweenSounds)
                    {
                        if(isImportant)
                        {
                            float volume = Mathf.Min(importantMaxVolume, collision.relativeVelocity.magnitude / velocityDivisor);
                            PlayHit(volume);
                        }
                        else
                        {
                            float volume = Mathf.Min(maxVolume, collision.relativeVelocity.magnitude / velocityDivisor);
                            soundEffectManager.RequestHitSound(this, volume);
                        }
                        
                    }

                    
                }
            }
        }
    }

    public void PlayHit(float volume)
    {
        AudioSource source = sources[Random.Range(0, sources.Length)];

        source.pitch = 1 + Random.Range(-pitchShiftMax, pitchShiftMax);
        source.volume = volume;
        source.Play();

        Debug.Log(source.pitch);

        timeSinceSound = 0;

        collisionCount++;
    }
}
