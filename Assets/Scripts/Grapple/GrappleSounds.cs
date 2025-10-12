using UnityEngine;

public class GrappleSounds : MonoBehaviour
{
    [SerializeField] GrappleHead grappleHead;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Vector2 pitchRange;
    GVar gvar;

    void Start()
    {
        gvar = GVar.Instance;
    }

    void Update()
    {
        audioSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, grappleHead.CurrentRopeLength / gvar.GrappleMaxDistance);
    } 

}
