using UnityEngine;
using UnityEngine.UI;

public class DashBarUI : MonoBehaviour
{
    Slider slider;
    GVar gvar;

    [Header("References")]
    [SerializeField] Image bar;
    [SerializeField] Image background;
    [SerializeField] PlayerController player;

    [Header ("Colors")]
    [SerializeField] Color chargeColor;
    [SerializeField] Color fullColor;
    [SerializeField] Color noDashBackgroundColor;
    [SerializeField] Color yesDashBackgroundColor;

    void Start()
    {
        slider = GetComponent<Slider>();
        gvar = GVar.Instance;
    }

    void Update()
    {
        // update the length of the bar to match the current rope length
        slider.value = (player.CurrentDashCharge / gvar.DashChargeTime) * 100;

        if (slider.value >= slider.maxValue)
        {
            bar.color = fullColor;
        }
        else
        {
            bar.color = chargeColor;
        }

        if (player.CanDash)
        {
            background.color = yesDashBackgroundColor;
        }
        else
        {
            background.color = noDashBackgroundColor;
        }
    }
}
