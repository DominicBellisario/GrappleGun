using UnityEngine;

public class DashSoundLogic : SoundLogic
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
