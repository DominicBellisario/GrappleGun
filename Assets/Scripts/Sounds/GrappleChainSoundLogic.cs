using Unity.VisualScripting;
using UnityEngine;

public class GrappleChainSoundLogic : MonoBehaviour
{
    [SerializeField] AudioSource grappleChainSource;
    [SerializeField] GrappleHead grappleHead;
    [SerializeField] AudioClip[] grappleChainClips;
    [SerializeField] float distanceToPlaySound;
    [SerializeField] Vector2 pitchRange;

    GVar gvar;
    float lastRecordedDistance = 0f;

    void Start()
    {
        gvar = GVar.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // play a random sound from the array when the grapple head moves a certain distance
        if (Mathf.Abs(grappleHead.CurrentRopeLength - lastRecordedDistance) >= distanceToPlaySound)
        {
            grappleChainSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, grappleHead.CurrentRopeLength / gvar.GrappleMaxDistance);
            grappleChainSource.PlayOneShot(grappleChainClips[Random.Range(0, grappleChainClips.Length)]);
            lastRecordedDistance = grappleHead.CurrentRopeLength;
        }
    }
}
