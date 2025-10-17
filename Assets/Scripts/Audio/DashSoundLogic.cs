using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DashSoundLogic : MonoBehaviour
{
    AudioSource dashSource;

    void Start()
    {
        dashSource = GetComponent<AudioSource>();
    }
    
    void OnEnable()
    {
        PlayerController.OnDashEvent += Play;
    }
    void OnDisable()
    {
        PlayerController.OnDashEvent -= Play;
    }

    public void Play()
    {
        dashSource.Play();
    }
}
