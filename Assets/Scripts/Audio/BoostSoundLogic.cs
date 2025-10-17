using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoostSoundLogic : SoundLogic
{
    [SerializeField] AudioClip boostStartClip;
    [SerializeField] AudioClip boostStopClip;
    [SerializeField] AudioClip boostEmptyClip;

    void OnEnable()
    {
        PlayerController.OnBoostStartEvent += () => PlayBoostStartSound();
        PlayerController.OnBoostStopEvent += () => PlayBoostStopSound();
        PlayerController.OnBoostEmptyEvent += () => PlayBoostEmptySound();
    }
    void OnDisable()
    {
        PlayerController.OnBoostStartEvent -= () => PlayBoostStartSound();
        PlayerController.OnBoostStopEvent -= () => PlayBoostStopSound();
        PlayerController.OnBoostEmptyEvent -= () => PlayBoostEmptySound();
    }

    private void PlayBoostStartSound()
    {
        audioSource.clip = boostStartClip;
        Play();
    }

    private void PlayBoostStopSound()
    {
        audioSource.clip = boostStopClip;
        Play();
    }
    private void PlayBoostEmptySound()
    {
        audioSource.clip = boostEmptyClip;
        Play();
    }
}
