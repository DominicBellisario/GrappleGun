using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundLogic : MonoBehaviour
{
    protected AudioSource audioSource;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
