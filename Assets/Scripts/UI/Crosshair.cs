using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color standardGrappleColor;
    [SerializeField] Color specialGrappleColor;

    Image crosshair;

    void Start()
    {
        crosshair = GetComponent<Image>();
    }

    public void UpdateCrosshair(Collider hitObject)
    {
        if (hitObject == null)
        {
            crosshair.color = defaultColor;
            return;
        }
        
        if (hitObject.gameObject.CompareTag("Normal Grap Surface"))
        {
            crosshair.color = standardGrappleColor;
        }
        else if (hitObject.gameObject.CompareTag("No Grap Surface"))
        {
            crosshair.color = defaultColor;
        }
        else
        {
            crosshair.color = specialGrappleColor;
        }
    }
}
