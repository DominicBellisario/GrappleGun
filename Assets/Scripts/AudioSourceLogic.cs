using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceLogic : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SwitchClip(int index)
    {
        audioSource.clip = audioClips[index];
    }
}
