using UnityEngine;

public class ReturnSoundLogic : SoundLogic
{
    void OnEnable()
    {
        GrappleHead.OnEndGrappleReturnEvent += (timer) => { if (timer != 0) Play(); };
    }
    void OnDisable()
    {
        GrappleHead.OnEndGrappleReturnEvent -= (timer) => { if (timer != 0) Play(); };
    }
}
