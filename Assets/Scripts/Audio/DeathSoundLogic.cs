using UnityEngine;

public class DeathSoundLogic : SoundLogic
{
    void OnEnable()
    {
        PlayerEvents.OnPlayerOutOfBounds += Play;
        PlayerEvents.OnPlayerDie += Play;
    }
    void OnDisable()
    {
        PlayerEvents.OnPlayerOutOfBounds -= Play;
        PlayerEvents.OnPlayerDie -= Play;
    }
}
