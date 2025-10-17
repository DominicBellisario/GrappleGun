using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoostSoundLogic : SoundLogic
{
    [SerializeField] AudioClip boostStartClip;
    [SerializeField] AudioClip boostStopClip;
    [SerializeField] AudioClip boostEmptyClip;

    public void PlayBoostStartSound()
    {
        audioSource.clip = boostStartClip;
        Play();
    }
    public void PlayBoostStopSound()
    {
        audioSource.clip = boostStopClip;
        Play();
    }
    public void PlayBoostEmptySound()
    {
        audioSource.clip = boostEmptyClip;
        Play();
    }
}
