using UnityEngine;
using UnityEngine.UI;

public class RopeBarUI : MonoBehaviour
{
    Slider slider;
    [SerializeField] Image bar;
    [SerializeField] GrappleHead grappleHead;

    [Header("Colors")]
    [SerializeField] Color idleColor;
    [SerializeField] Color maxColor;
    [SerializeField] Color hitColor;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = grappleHead.maxDistance;
    }

    void Update()
    {
        // update the length of the bar to match the current rope length
        slider.value = grappleHead.CurrentRopeLength;

        // do not change color if grapple is not in use
        if (grappleHead.CurrentRopeLength < 0.25f) return;

        // change bar color according to the length of the rope 
        else if (!grappleHead.IsAttached)
        {
            ChangeImageColor(bar, Color.Lerp(idleColor, maxColor, Mathf.Clamp(grappleHead.CurrentRopeLength / grappleHead.maxDistance, 0, 1)));
        }
        // if attatched, make the fill image the hit color
        else
        {
            ChangeImageColor(bar, Color.Lerp(idleColor, hitColor, Mathf.Clamp(grappleHead.CurrentRopeLength / grappleHead.maxDistance, 0, 1)));
        }
    }

    private void ChangeImageColor(Image image, Color color)
    {
        image.color = color;
    }
}
