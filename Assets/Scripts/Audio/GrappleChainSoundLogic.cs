using UnityEngine;

public class GrappleChainSoundLogic : SoundLogic
{
    [SerializeField] GrappleHead grappleHead;
    [SerializeField] AudioClip[] grappleChainClips;
    [SerializeField] float distanceToPlaySound;
    [SerializeField] Vector2 pitchRange;

    GVar gvar;
    float lastRecordedDistance = 0f;

    protected override void Start()
    {
        base.Start();
        gvar = GVar.Instance;
    }

    void Update()
    {
        // play a random sound from the array when the grapple head moves a certain distance
        if (Mathf.Abs(grappleHead.CurrentRopeLength - lastRecordedDistance) >= distanceToPlaySound)
        {
            audioSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, grappleHead.CurrentRopeLength / gvar.GrappleMaxDistance);
            audioSource.PlayOneShot(grappleChainClips[Random.Range(0, grappleChainClips.Length)]);
            lastRecordedDistance = grappleHead.CurrentRopeLength;
        }
    }
}
