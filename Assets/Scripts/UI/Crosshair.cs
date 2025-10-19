using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Crosshair : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color defaultGrappleColor;
    [SerializeField] Color standardGrappleColor;
    [SerializeField] Color specialGrappleColor;

    Image crosshair;

    void Start()
    {
        crosshair = GetComponent<Image>();
    }

    void OnEnable()
    {
        GrappleRangeRaycast.OnDetectedSurfaceChange += UpdateCrosshair;
    }
    void OnDisable()
    {
        GrappleRangeRaycast.OnDetectedSurfaceChange += UpdateCrosshair;
    }

    private void UpdateCrosshair(string tag)
    {
        if (tag == "Normal Grap Surface")
        {
            crosshair.color = standardGrappleColor;
        }
        else if (tag == "Reel" || tag == "Bird")
        {
            crosshair.color = specialGrappleColor;
        }
        else
        {
            crosshair.color = defaultGrappleColor;
        }
    }
}
