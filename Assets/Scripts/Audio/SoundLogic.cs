using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundLogic : MonoBehaviour
{
    protected AudioSource audioSource;
    protected float startingVolume;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startingVolume = audioSource.volume;
    }

    protected virtual void Play(RaycastHit unusedHit = default)
    {
        Play();
    }
    protected virtual void Play()
    {
        audioSource.Play();
    }
    
}
