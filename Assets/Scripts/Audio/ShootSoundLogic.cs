using UnityEngine;

public class ShootSoundLogic : SoundLogic
{
    void OnEnable()
    {
        PlayerController.OnShootGrappleEvent += (raycastHit) => Play();
    }
    void OnDisable()
    {
        PlayerController.OnShootGrappleEvent -= (raycastHit) => Play();
    }
}
