using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceLogic : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void Constructor(AudioClip clip, bool destroyAfterPlay = true)
    {
        audioSource.clip = clip;
        audioSource.Play();
        if (destroyAfterPlay) Destroy(gameObject, clip.length);
    }
}
