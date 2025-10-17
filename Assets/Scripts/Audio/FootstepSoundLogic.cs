using UnityEngine;

public class FootstepSoundLogic : MonoBehaviour
{
    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioClip[] footstepClips;
    int lastClipIndex = -1;
    
    public void PlayFootstepSound()
    {
        lastClipIndex++;
        if (lastClipIndex >= footstepClips.Length)
            lastClipIndex = 0;
        footstepSource.clip = footstepClips[lastClipIndex];
        footstepSource.Play();
    }
}
