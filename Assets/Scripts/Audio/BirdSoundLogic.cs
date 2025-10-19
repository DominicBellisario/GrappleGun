using UnityEngine;

public class BirdSoundLogic : SoundLogic
{
    void OnEnable()
    {
        PlayerController.OnDashEvent += Play;
    }
    void OnDisable()
    {
        PlayerController.OnDashEvent -= Play;
    }
}
