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

    [Header("Visuals")]
    [SerializeField] float smoothSpeed;

    //the current dash charge for the player
    float targetValue;

    void Start()
    {
        slider = GetComponent<Slider>();
        gvar = GVar.Instance;
        targetValue = (player.CurrentDashCharge / gvar.DashChargeTime) * 100;
    }

    void Update()
    {
        // lerp the bar to follow the current dash charge
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);

        //update dash charge if it is different
        if (targetValue != (player.CurrentDashCharge / gvar.DashChargeTime) * 100)
        {
            targetValue = (player.CurrentDashCharge / gvar.DashChargeTime) * 100;
        }

        // change the bars color if it reaches the max
        if (targetValue >= slider.maxValue)
        {
            bar.color = fullColor;
        }
        else
        {
            bar.color = chargeColor;
        }

        //change the background if dash is depleted
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
