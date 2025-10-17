using UnityEngine;

public class FootstepSoundLogic : SoundLogic
{
    [SerializeField] AudioClip[] footstepClips;
    int lastClipIndex = -1;
    
    protected override void Play()
    {
        lastClipIndex++;
        if (lastClipIndex >= footstepClips.Length)
            lastClipIndex = 0;
        audioSource.clip = footstepClips[lastClipIndex];
        base.Play();
    }
}
