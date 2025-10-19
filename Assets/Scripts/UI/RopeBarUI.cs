using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RopeBarUI : MonoBehaviour
{
    Slider slider;
    GVar gvar;
    [SerializeField] Image bar;

    [Header("Colors")]
    [SerializeField] Color idleColor;
    [SerializeField] Color maxColor;
    [SerializeField] Color hitColor;

    bool isGrappleAttached;

    void Start()
    {
        gvar = GVar.Instance;
        slider = GetComponent<Slider>();
        slider.maxValue = gvar.GrappleMaxDistance;
        isGrappleAttached = false;
    }

    void OnEnable()
    {
        GrapplePhysics.OnGrappleAttatched += HandleOnGrappleAttatched;
        GrapplePhysics.OnGrappleReleased += HandleOnGrappleReleased;
    }
    void OnDisable()
    {
        GrapplePhysics.OnGrappleAttatched -= HandleOnGrappleAttatched;
        GrapplePhysics.OnGrappleReleased -= HandleOnGrappleReleased;
    }
    void HandleOnGrappleAttatched() { isGrappleAttached = true; }
    void HandleOnGrappleReleased() { isGrappleAttached = false; }

    void Update()
    {
        // update the length of the bar to match the current rope length
        slider.value = gvar.CurrentRopeLength;

        // do not change color if grapple is not in use
        if (gvar.CurrentRopeLength < 0.25f) return;

        // change bar color according to the length of the rope 
        else if (!isGrappleAttached)
        {
            ChangeImageColor(bar, Color.Lerp(idleColor, maxColor, Mathf.Clamp(gvar.CurrentRopeLength / gvar.GrappleMaxDistance, 0, 1)));
        }
        // if attatched, make the fill image the hit color
        else
        {
            ChangeImageColor(bar, Color.Lerp(idleColor, hitColor, Mathf.Clamp(gvar.CurrentRopeLength / gvar.GrappleMaxDistance, 0, 1)));
        }
    }
    
    private void ChangeImageColor(Image image, Color color)
    {
        image.color = color;
    }
}
