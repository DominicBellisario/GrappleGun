public class GunShootSoundLogic : SoundLogic
{
    void OnEnable()
    {
        PlayerController.OnShootGunEvent += Play;
    }
    void OnDisable()
    {
        PlayerController.OnShootGunEvent -= Play;
    }
}
