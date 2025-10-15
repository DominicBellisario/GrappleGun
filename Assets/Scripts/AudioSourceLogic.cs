using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceLogic : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void Constructor(AudioClip clip, float pitch = 1.0f, float volume = 1.0f, bool destroyAfterPlay = true)
    {
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
        if (destroyAfterPlay) Destroy(gameObject, clip.length);
    }
}
