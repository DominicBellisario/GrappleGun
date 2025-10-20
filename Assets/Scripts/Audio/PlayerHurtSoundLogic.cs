using UnityEngine;

public class PlayerHurtSoundLogic : SoundLogic
{
    void OnEnable()
    {
        PlayerEvents.OnPlayerDecreaseHealth += Play;
    }
    void OnDisable()
    {
        PlayerEvents.OnPlayerDecreaseHealth -= Play;
    }
}
