using UnityEngine;

public class BoostSoundLogic : MonoBehaviour
{
    [SerializeField] AudioSource boostSource;
    [SerializeField] AudioClip boostStartClip;
    [SerializeField] AudioClip boostStopClip;
    [SerializeField] AudioClip boostEmptyClip;

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
