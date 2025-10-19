using System.Runtime.CompilerServices;
using UnityEngine;

public class FootstepSoundLogic : SoundLogic
{
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] AudioClip landingClip;
    [SerializeField] float minVolume;
    [SerializeField] float speedNeededForMaxLandingNoiseVolume;
    
    int lastClipIndex = -1;

    void OnEnable()
    {
        CameraBob.OnWalkCycleComplete += Play;
        PlayerController.OnGroundedEvent += PlayLandingSound;
    }
    void OnDisable()
    {
        CameraBob.OnWalkCycleComplete -= Play;
    }

    protected override void Play()
    {
        // reset the volume
        audioSource.volume = startingVolume;

        // switch to next footstep clip
        lastClipIndex++;
        if (lastClipIndex >= footstepClips.Length)
            lastClipIndex = 0;

        audioSource.PlayOneShot(footstepClips[lastClipIndex]);
        
    }
    
    private void PlayLandingSound(float verticalSpeed)
    {
        // adjust the volume based on the player's landing speed
        audioSource.volume = Mathf.Clamp(-verticalSpeed / speedNeededForMaxLandingNoiseVolume, minVolume, 1f);
        Debug.Log(-verticalSpeed / speedNeededForMaxLandingNoiseVolume);
        audioSource.PlayOneShot(landingClip);
        
    }
}
