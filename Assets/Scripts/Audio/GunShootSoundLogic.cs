using UnityEngine;

public class GunShootSoundLogic : SoundLogic
{
    [SerializeField] AudioClip reloadClip;

    void OnEnable()
    {
        PlayerController.OnShootGunEvent += Play;
        PlayerController.OnGunReloadedEvent += PlayReloadSound;
    }
    void OnDisable()
    {
        PlayerController.OnShootGunEvent -= Play;
        PlayerController.OnGunReloadedEvent -= PlayReloadSound;
    }

    private void PlayReloadSound()
    {
        audioSource.PlayOneShot(reloadClip);
    }
}
