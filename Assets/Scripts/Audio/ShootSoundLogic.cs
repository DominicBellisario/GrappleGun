public class ShootSoundLogic : SoundLogic
{
    void OnEnable()
    {
        PlayerController.OnShootGrappleEvent += Play;
    }

    void OnDisable()
    {
        PlayerController.OnShootGrappleEvent -= Play;
    }
}
