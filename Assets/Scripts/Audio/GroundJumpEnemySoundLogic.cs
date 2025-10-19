using UnityEngine;
using UnityEngine.Video;

public class GroundJumpEnemySoundLogic : SoundLogic
{
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip landClip;

    public void PlayAttackClip()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(attackClip);
    }
    
    public void PlayLandClip()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(landClip);
    }
}
