using UnityEngine;

public class ReturnSoundLogic : SoundLogic
{
    void OnEnable()
    {
        GrappleHead.OnEndGrappleReturnEvent += PlayReturnSound;
    }
    void OnDisable()
    {
        GrappleHead.OnEndGrappleReturnEvent -= PlayReturnSound;
    }

    private void PlayReturnSound(float timer)
    {
        if (timer != 0) Play();
    }
}
