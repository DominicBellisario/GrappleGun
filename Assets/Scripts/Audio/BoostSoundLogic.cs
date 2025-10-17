using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoostSoundLogic : MonoBehaviour
{
    AudioSource boostSource;
    [SerializeField] AudioClip boostStartClip;
    [SerializeField] AudioClip boostStopClip;
    [SerializeField] AudioClip boostEmptyClip;

    void Start()
    {
        boostSource = GetComponent<AudioSource>();
    }

    public void PlayBoostStartSound()
    {
        boostSource.clip = boostStartClip;
        boostSource.Play();
    }
    public void PlayBoostStopSound()
    {
        boostSource.clip = boostStopClip;
        boostSource.Play();
    }
    public void PlayBoostEmptySound()
    {
        boostSource.clip = boostEmptyClip;
        boostSource.Play();
    }
}
