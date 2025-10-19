using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Crosshair : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color noGrappleColor;
    [SerializeField] Color normalGrappleColor;
    [SerializeField] Color enemyColor;
    [SerializeField] Color specialSurfaceColor;

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
        GrappleRangeRaycast.OnDetectedSurfaceChange -= UpdateCrosshair;
    }

    private void UpdateCrosshair(string tag)
    {
        if (tag == "Normal Grap Surface")
        {
            crosshair.color = normalGrappleColor;
        }
        else if (tag == "Reel" || tag == "Bird")
        {
            crosshair.color = specialSurfaceColor;
        }
        else if (tag == "Enemy")
        {
            crosshair.color = enemyColor;
        }
        else
        {
            crosshair.color = noGrappleColor;
        }
    }
}
